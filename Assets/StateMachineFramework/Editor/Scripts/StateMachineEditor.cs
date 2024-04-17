using StateMachineFramework.Runtime;
using UnityEditor;
using UnityEngine.UIElements;


namespace StateMachineFramework.Editor {
    [CustomEditor(typeof(StateMachine))]
    public class StateMachineInspector : UnityEditor.Editor {
        public VisualTreeAsset uxml;
        static Window window;
        public override VisualElement CreateInspectorGUI() {
            base.CreateInspectorGUI();
            var ve = uxml.Instantiate();
            ve.Q<Button>().clicked += OpenWindow;
            return ve;
        }

        private void OpenWindow() {
            if (window == null) {
                MakeInstance();
            }

            window.Show();
            window.Focus();
        }
        void MakeInstance() {
            foreach (var win in Window.windowsList) {
                if (win.stateMachine) {
                    if (win.stateMachine.GetInstanceID() == target.GetInstanceID()) {
                        window = win;
                        return;
                    }
                }
            }

            window = EditorWindow.CreateInstance<Window>();
            window.HardInit(target as StateMachine);
        }
    }
}