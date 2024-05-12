using StateMachineFramework.Runtime;
using UnityEditor;
using UnityEngine;

namespace StateMachineFramework.Editor {
    public class MainSMWindow : SMWindow {

        [MenuItem("Window/State Machine Framework")]
        public static void ShowMyEditor() {
            MainSMWindow wnd = GetWindow<MainSMWindow>();
            wnd.minSize = new Vector2(450, 200);
            wnd.titleContent = new GUIContent("State Machine", wnd.icon);
        }

        public override void CreateGUI() {
            base.CreateGUI();
            UnityEditor.Selection.selectionChanged += SelectionUpdated;

        }
        void SelectionUpdated() {
            editor.SetDisplay(UnityEditor.Selection.activeGameObject?.GetComponent<StateMachine>());
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            UnityEditor.Selection.selectionChanged -= SelectionUpdated;

        }
    }
}

