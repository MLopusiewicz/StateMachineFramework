using StateMachineFramework.Runtime;
using UnityEditor;
using UnityEngine.UIElements;


namespace StateMachineFramework.Editor {
    [CustomEditor(typeof(Runtime.StateMachine))]
    public class SMFInspector : UnityEditor.Editor {
        public VisualTreeAsset uxml;
        static TargetSMWindow window; 
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
            foreach (var win in TargetSMWindow.windowsList) {
                if (win.editor.stateMachine) {
                    if (win.editor.stateMachine.GetInstanceID() == target.GetInstanceID()) {
                        window = win;
                        return;
                    }
                }
            }

            window = EditorWindow.CreateInstance<TargetSMWindow>();
            window.Init(target as Runtime.StateMachine);
        }
    }
}