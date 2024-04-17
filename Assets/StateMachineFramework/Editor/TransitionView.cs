using GluonGui;
using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace StateMachineFramework.Editor {

    public class TransitionView {

        VisualElement container;
        Window w;

        Dictionary<Transition, TransitionVE> ves = new();
        Dictionary<TransitionVE, List<Transition>> trans = new();
        List<Transition> selectedTransitions = new();

        public TransitionView(Window w) {
            this.w = w;
            w.rootVisualElement.Q<ViewPortVE>().RegisterCallback<KeyDownEvent>(OnKeyDown);
            container = w.rootVisualElement.Q(name: "LineContainer");
            w.depthPanel.OnDepthChanged += (t) => Redraw();
            w.selection.OnSelectionCleared += ClearSelection;
            Undo.undoRedoPerformed += Redraw;
        }

        public void ClearSelection() {
            foreach (var a in selectedTransitions)
                ves[a].RemoveFromClassList("selected");
            selectedTransitions.Clear();

            w.transitionInspector.Clear();
        }

        public void Update() {
            foreach (var a in trans.Keys) {
                a.Update();
            }
        }

        public void Redraw() {
            container.Clear();
            ves.Clear();
            trans.Clear();
            selectedTransitions.Clear();

            foreach (var transition in w.stateMachine.anyState.transitions) {
                if (w.depthPanel.IsInScope(transition.target))
                    CreateTransition(transition);
            }

            foreach (var transition in w.depthPanel.ActiveTree.transitions)
                if (w.depthPanel.IsInScope(transition.target))
                    CreateTransition(transition, w.depthPanel.ActiveTree.enterNode, transition.target);

            foreach (var nodeInScope in w.depthPanel.ActiveTree.nodes) {

                foreach (var transition in nodeInScope.transitions) {
                    var targetInScope = w.depthPanel.IsInScope(transition.target);
                    if (targetInScope)
                        CreateTransition(transition);
                    if (transition.target == w.depthPanel.ActiveTree) {
                        CreateTransition(transition, transition.source, w.depthPanel.ActiveTree.exitNode);
                    }
                }
            }
        }

        public void Clear() {
            container.Clear();
        }



        void OnKeyDown(KeyDownEvent evt) {
            if (evt.keyCode == KeyCode.Delete) {
                foreach (var a in selectedTransitions)
                    w.serialization.RemoveTransition(a);
                w.serialization.Apply();
                Redraw();
            }
        }


        Transition GetDuplicate(Transition t) {
            foreach (var a in ves.Keys)
                if (t.source == a.source)
                    if (t.target == a.target)
                        return a;
            return null;
        }

        TransitionVE CreateTransition(Transition t) {
            return CreateTransition(t, t.source, t.target);
        }

        TransitionVE CreateTransition(Transition t, Node source, Node target) {

            var d = GetDuplicate(t);

            if (d != null) {
                ves[d].AddTransitionCount();
                trans[ves[d]].Add(t);
                ves.Add(t, ves[d]);
                return ves[d];
            }

            var l = new TransitionVE();
            if (!trans.ContainsKey(l))
                trans.Add(l, new List<Transition>());
            trans[l].Add(t);

            ves.Add(t, l);

            container.Add(l);
            l.Init(w.nodeView.nodes[source],
                   w.nodeView.nodes[target],
                   w.nodeView.nodeContainer);


            l.OnSelected += TransitionClicked;
            l.AddManipulator(
                new ContextualMenuManipulator((evt) => {
                    var p = evt.localMousePosition;
                    evt.menu.ClearItems();
                    evt.menu.AppendAction("Remove transition", (x) => RemoveTransition(t), DropdownMenuAction.AlwaysEnabled);
                    evt.StopPropagation();
                }));


            return l;
        }

        void TransitionClicked(MouseDownEvent x, TransitionVE vE) {
            if (x.button != 0)
                return;
            foreach (var a in selectedTransitions)
                ves[a].RemoveFromClassList("selected");

            if (x.shiftKey) {
                foreach (var a in trans[vE]) {
                    if (selectedTransitions.Contains(a))
                        selectedTransitions.Remove(a);
                    else
                        selectedTransitions.Add(a);
                }
            } else {
                selectedTransitions.Clear();
                foreach (var a in trans[vE]) {
                    selectedTransitions.Add(a);
                }
            }

            foreach (var a in selectedTransitions)
                ves[a].AddToClassList("selected");
            w.transitionInspector.Display(selectedTransitions);

            w.nodeView.ClearSelection();
        }

        void RemoveTransition(Transition t) {
            w.serialization.RemoveTransition(t);
            w.serialization.Apply();
            Redraw();
        }

    }
}