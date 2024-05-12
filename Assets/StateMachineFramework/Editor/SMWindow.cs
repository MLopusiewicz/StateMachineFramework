using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {

    [ExecuteAlways]
    public class SMWindow : EditorWindow {
        [SerializeField] private int m_SelectedIndex = -1;
        public VisualTreeAsset asset;
        public Texture icon;

        public static List<SMWindow> windowsList = new List<SMWindow>();


        public StateMachineEditor editor;

        public void Init(StateMachine sm) {
            this.titleContent = new GUIContent(sm.gameObject.name, this.icon);
            editor.stateMachine = sm;
        }

        public virtual void CreateGUI() {
            Debug.Log("GUI done");
            windowsList.Add(this);
            Focus();
            var editorTree = asset.Instantiate();
            EditorApplication.playModeStateChanged += OnPlayChanged;
            rootVisualElement.Add(editorTree);

            editorTree.style.width = new Length(100, LengthUnit.Percent);
            editorTree.style.height = new Length(100, LengthUnit.Percent);

            editor = new StateMachineEditor(rootVisualElement);

        }

        private void OnPlayChanged(PlayModeStateChange change) {
            if (change == PlayModeStateChange.ExitingEditMode) {
                editor.isRuntime = true;
                editor.runtime.Init();
            } else {

                editor.runtime.Clear();
                editor.isRuntime = false;
            }
        }



        public void Update() {
            if (Application.isPlaying) {
                editor.runtime?.Update();
            } else {
                editor.transitions.Update();
            }
        }

        protected virtual void OnDestroy() {
            Debug.Log("Destroyed");
            windowsList.Remove(this);
        }
    }
}

