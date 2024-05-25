using Codice.CM.Common.Replication;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {

    [ExecuteAlways]
    public abstract class SMWindow : EditorWindow {
        [SerializeField] private int m_SelectedIndex = -1;
        public VisualTreeAsset asset;
        public Texture icon;



        public StateMachineEditor editor;


        private void OnFocus() {
            if (editor != null) {
                if (editor.stateMachine != null)
                    editor.unredo.Redraw();

            }
        }

        public virtual void CreateGUI() {
            Focus();
            var editorTree = asset.Instantiate();
            EditorApplication.playModeStateChanged += OnPlayChanged;
            rootVisualElement.Add(editorTree);
            editorTree.style.width = new Length(100, LengthUnit.Percent);
            editorTree.style.height = new Length(100, LengthUnit.Percent);
            editor = new StateMachineEditor(rootVisualElement);
            SetPlayMode(Application.isPlaying);
        }

        private void OnPlayChanged(PlayModeStateChange change) {
            SetPlayMode(change == PlayModeStateChange.ExitingEditMode);
        }



        public void Update() {
            if (Application.isPlaying) {
                editor.runtime?.Update();
            } else {
                editor.transitions.Update();
            }
        }

        public void SetPlayMode(bool state) {
            if (state) {
                editor.isRuntime = true;
                editor.runtime.Init();
            } else {
                editor.runtime.Clear();
                editor.isRuntime = false;
            }
        }
    }
}
