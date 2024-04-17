using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.Client.Common.DiffMergeToolConfig;

namespace StateMachineFramework.Editor {

    [ExecuteAlways]
    public class Window : EditorWindow {
        [SerializeField] private int m_SelectedIndex = -1;
        public VisualTreeAsset asset;
        public Texture icon;

        public static List<Window> windowsList = new List<Window>();
        [MenuItem("Window/State Machine Framework")]
        public static void ShowMyEditor() {
            Window wnd = GetWindow<Window>();
            wnd.minSize = new Vector2(450, 200);
            wnd.titleContent = new GUIContent("State Machine", wnd.icon);
            UnityEditor.Selection.selectionChanged += wnd.SelectionUpdated;
        }

        public SerializationHelper serialization;
        public ParameterInspector parameterTab;
        public NodeTreeView nodeView;
        public NodeInspector nodeInspector;
        public TransitionInspector transitionInspector;
        public TransitionMaker transitionMaker;
        public TransitionView transitions;

        public DepthPanel depthPanel;
        public StateMachine stateMachine;
        public EditorSelection selection;

        public RuntimeDisplay runtime;
        public bool isRuntime = false;
        public void HardInit(StateMachine sm) {
            this.titleContent = new GUIContent(sm.gameObject.name, this.icon);
            stateMachine = sm;
        }

        public void SelectionUpdated() {
            stateMachine = UnityEditor.Selection.activeGameObject?.GetComponent<StateMachine>();
            Refresh();
        }

        public void CreateGUI() {
            windowsList.Add(this);
            Focus();
            var editorTree = asset.Instantiate();
            EditorApplication.playModeStateChanged += OnPlayChanged;

            rootVisualElement.Add(editorTree);

            editorTree.style.width = new Length(100, LengthUnit.Percent);
            editorTree.style.height = new Length(100, LengthUnit.Percent);
            serialization = new SerializationHelper();
            selection = new EditorSelection(this);
            depthPanel = new DepthPanel(this);

            transitionMaker = new TransitionMaker(this);
            parameterTab = new ParameterInspector(this);
            nodeView = new NodeTreeView(this);
            nodeInspector = new NodeInspector(this);
            transitions = new TransitionView(this);
            transitionInspector = new TransitionInspector(this);

            Refresh();
        }

        private void OnPlayChanged(PlayModeStateChange change) {
            if (change == PlayModeStateChange.ExitingEditMode) {
                isRuntime = true;
                runtime.Init();
            } else {

                runtime.Clear();
                isRuntime = false;
            }
            Refresh();
        }

        public void Refresh() {

            if (stateMachine == null) {
                parameterTab.Clear();
                nodeView.Clear();
                nodeInspector.Clear();
                transitionInspector.Clear();
                transitions.Clear();
                return;
            }

            depthPanel.Init();
            serialization.Init(this.stateMachine);
            parameterTab.Init();
            nodeView.Init(stateMachine.Root);
            transitions.Redraw();

            runtime = new RuntimeDisplay(this);
        }

        public void Update() {
            if (Application.isPlaying) {
                runtime?.Update();
            } else {
                transitions.Update();
            }
        }

        private void OnDestroy() {
            UnityEditor.Selection.selectionChanged -= Refresh;
            windowsList.Remove(this);
        }

    }
}