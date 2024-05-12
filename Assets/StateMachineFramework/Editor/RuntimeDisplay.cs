using StateMachineFramework.Runtime;
namespace StateMachineFramework.Editor {

    public class RuntimeDisplay {
        private StateMachineEditor editor;

        public RuntimeDisplay(StateMachineEditor editor) {
            this.editor = editor;

        }
        public void Init() {
            editor.stateMachine.logic.OnNodeEnter += Highlight;
            editor.stateMachine.logic.OnNodeExit += RemoveHighlight;
            editor.depthPanel.OnDepthChanged += DepthChanged;
            DepthChanged(editor.depthPanel.ActiveTree);
        }
        public void Clear() {
            editor.stateMachine.logic.OnNodeEnter -= Highlight;
            editor.stateMachine.logic.OnNodeExit -= RemoveHighlight;
            editor.depthPanel.OnDepthChanged -= DepthChanged;

        }
        private void DepthChanged(TreeNode tree) {

            foreach (var a in editor.stateMachine.logic.activeNodes) {
                if (editor.depthPanel.IsInScope(a))
                    editor.nodeView.nodes[a].AddToClassList("active");
            }
        }

        private void Highlight(Node node) {
            if (editor.depthPanel.IsInScope(node))
                editor.nodeView.nodes[node].AddToClassList("active");
        }
        private void RemoveHighlight(Node node) {
            if (editor.depthPanel.IsInScope(node))
                editor.nodeView.nodes[node].RemoveFromClassList("active");

        }

        public void Update() {
            //Debug.Log(w.stateMachine.logic.activeNodes.Count); 
        }
    }
}