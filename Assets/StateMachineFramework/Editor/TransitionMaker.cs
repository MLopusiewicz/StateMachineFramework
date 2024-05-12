using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class TransitionMaker {
        public bool isPicking { get; private set; }
        Node target, source;
        TransitionVE pickingLine;
        ViewPortVE viewport;
        StateMachineEditor editor;
        VisualElement container;

        public TransitionMaker(StateMachineEditor editor) {
            this.editor = editor;
            viewport = editor.rootVisualElement.Q<ViewPortVE>();
            container = editor.rootVisualElement.Q(name: "LineContainer");

            viewport.RegisterCallback<MouseDownEvent>(OnMouseDown);
            viewport.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt) {
            if (evt.keyCode == UnityEngine.KeyCode.Escape) {
                DisablePickingState();
            }
        }

        void OnMouseDown(MouseDownEvent evt) {
            if (evt.target is not NodeVE) {
                DisablePickingState();
                return;
            }
        }

        void OnMouseMove(MouseMoveEvent evt) {
            var p = viewport.contentContainer.WorldToLocal(evt.mousePosition);
            pickingLine.Init(editor.nodeView.nodes[source].localBound.center, p);
        }

        public void Start(Node n) {
            if (n == null)
                return;
            pickingLine = new TransitionVE();
            container.Add(pickingLine);
            source = n;
            viewport.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            isPicking = true;
        }

        void DisablePickingState() {
            target = null;
            source = null;
            pickingLine?.SetDisplay(false);
            isPicking = false;
            viewport.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        public void MakeTransition(Node targetNode) {
            target = targetNode;

            var ss = source;
            if (target == ss)
                return;

            if (target is SpecialNode specialNode) {
                if (!specialNode.canBeTarget) {
                    DisablePickingState();
                    return;
                }
                if (specialNode.name == "Exit") {
                    target = editor.depthPanel.ActiveTree;
                }
            }

            if (ss is SpecialNode special) {
                if (!special.canBeSource) {
                    DisablePickingState();
                    return;
                }
                if (special.name == "Enter") {
                    ss = editor.depthPanel.ActiveTree;
                }
            }

            Transition trans = new Transition() { source = ss, target = target };

            editor.serialization.AddTransition(trans);
            editor.serialization.Apply();
            DisablePickingState();
            editor.transitions.Redraw();
        }
    }
}