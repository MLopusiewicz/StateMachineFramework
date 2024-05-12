using StateMachineFramework.Editor;
using StateMachineFramework.Runtime;
using UnityEngine.UIElements;

public class StateMachineEditor {
    public StateMachine stateMachine;

    public SerializationHelper serialization;
    public ParameterInspector parameterTab;
    public NodeTreeView nodeView;
    public NodeInspector nodeInspector;
    public TransitionInspector transitionInspector;
    public TransitionMaker transitionMaker;
    public TransitionView transitions;
    public Unredo unredo;

    public DepthPanel depthPanel;
    public EditorSelection selection;
    public InspectorWindow inspector;
    public RuntimeDisplay runtime;

    public VisualElement rootVisualElement;
    public bool isRuntime = false;

    public StateMachineEditor(VisualElement rootVisualElement) {
        this.rootVisualElement = rootVisualElement;
        serialization = new SerializationHelper();
        selection = new EditorSelection(this);
        depthPanel = new DepthPanel(this);
        inspector = new InspectorWindow(this);
        transitionMaker = new TransitionMaker(this);
        parameterTab = new ParameterInspector(this);
        nodeView = new NodeTreeView(this);
        nodeInspector = new NodeInspector(this);
        transitions = new TransitionView(this);
        transitionInspector = new TransitionInspector(this);
        unredo = new Unredo(this);

        runtime = new RuntimeDisplay(this);
    }

    public void SetDisplay(StateMachine sm) {
        stateMachine = sm;

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

    }
    public void Dispose() {
        unredo.Dispose();
    }
}