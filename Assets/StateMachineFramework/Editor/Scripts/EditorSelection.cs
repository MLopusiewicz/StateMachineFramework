using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class EditorSelection {
        ViewPortVE viewPort;
        public event Action OnSelectionCleared;
        public event Action<Node> OnNodeSelected;

        Window w;

        public EditorSelection(Window w) {
            viewPort = w.rootVisualElement.Q<ViewPortVE>();
            viewPort.RegisterCallback<MouseDownEvent>(OnClicked);
            this.w = w;
        }

        void OnClicked(MouseDownEvent e) {
            if (e.target == viewPort) {
                if (e.button == 0)
                    ClearSelection();
            }
        }

        private void ClearSelection() {
            OnSelectionCleared?.Invoke();
        }


    }
}