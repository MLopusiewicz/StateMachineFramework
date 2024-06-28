using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace StateMachineFramework.Editor {

    public class TransitionView {

        VisualElement container;
        StateMachineEditor editor;

        Dictionary<Transition, TransitionVE> ves = new();
        Dictionary<TransitionVE, List<Transition>> trans = new();
        //List<Transition> selectedTransitions = new();
        SerializedSelection<Transition> selection => editor.selection.transitions;

        public TransitionView(StateMachineEditor editor) {
            this.editor = editor;
            editor.rootVisualElement.Q<ViewPortVE>().RegisterCallback<KeyDownEvent>(OnKeyDown);
            container = editor.rootVisualElement.Q(name: "LineContainer");
            editor.depthPanel.OnDepthChanged += (t) => Redraw();
            editor.selection.OnSelectionCleared += ClearSelection; 
        }

        public void ClearSelection() {
            foreach (var a in selection.GetItems())
                ves[a].RemoveFromClassList("selected");
            selection.Clear();
            editor.transitionInspector.Clear();
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

            foreach (var transition in editor.stateMachine.AnyState.transitions) {
                if (editor.depthPanel.IsInScope(transition.target))
                    CreateTransition(transition);
            }

            foreach (var transition in editor.depthPanel.ActiveTree.transitions)
                if (editor.depthPanel.IsInScope(transition.target))
                    CreateTransition(transition, editor.depthPanel.ActiveTree.enterNode, transition.target);

            foreach (var nodeInScope in editor.depthPanel.ActiveTree.nodes) {

                foreach (var transition in nodeInScope.transitions) {
                    var targetInScope = editor.depthPanel.IsInScope(transition.target);
                    if (targetInScope)
                        CreateTransition(transition);
                    if (transition.target == editor.depthPanel.ActiveTree) {
                        CreateTransition(transition, transition.source, editor.depthPanel.ActiveTree.exitNode);
                    }
                }
            }

            DrawSelection();
        }

        public void Clear() {
            container.Clear();
        }



        void OnKeyDown(KeyDownEvent evt) {
            if (evt.keyCode == KeyCode.Delete) {
                foreach (var a in selection.GetItems())
                    editor.serialization.RemoveTransition(a);
                editor.serialization.Apply();
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
            l.Init(editor.nodeView.nodes[source],
                   editor.nodeView.nodes[target],
                   editor.nodeView.nodeContainer);


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
            foreach (var a in selection.GetItems())
                ves[a].RemoveFromClassList("selected");

            if (x.shiftKey) {
                foreach (var a in trans[vE]) {
                    if (selection.Contains(a))
                        selection.Remove(a);
                    else
                        selection.Add(a);
                }
            } else {
                selection.Clear();
                foreach (var a in trans[vE]) {
                    selection.Add(a);
                }
            }

            DrawSelection();
            editor.transitionInspector.Display(selection.GetItems());

            editor.nodeView.ClearSelection();
        }
        void DrawSelection() {
            foreach (var a in selection.GetItems())
                ves[a].AddToClassList("selected");
        }

        void RemoveTransition(Transition t) {
            editor.serialization.RemoveTransition(t);
            editor.serialization.Apply();
            Redraw();
        }

    }
}