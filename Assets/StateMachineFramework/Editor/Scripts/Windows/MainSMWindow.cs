using UnityEditor;
using UnityEngine;

namespace StateMachineFramework.Editor {
    public class MainSMWindow : SMWindow {

        [MenuItem("Window/State Machine Framework")]
        public static void GetWindow() {
            MainSMWindow wnd = GetWindow<MainSMWindow>();
            wnd.minSize = new Vector2(450, 200);
            wnd.titleContent = new GUIContent("State Machine", wnd.icon);
        }

        public override void CreateGUI() {
            base.CreateGUI();
            UnityEditor.Selection.selectionChanged += SelectionUpdated;
        }
        void SelectionUpdated() {
            var g = UnityEditor.Selection.GetFiltered<GameObject>(SelectionMode.Assets);
            foreach (var a in g) {
                var s = a.GetComponentInChildren<Runtime.StateMachine>();
                if (s != null) {
                    editor.SetDisplay(s);
                    return;
                }
            }

            //editor.SetDisplay(UnityEditor.Selection.activeGameObject?.GetComponent<Runtime.StateMachine>());
        }

        protected void OnDestroy() {
            UnityEditor.Selection.selectionChanged -= SelectionUpdated;

        }
    }
}

