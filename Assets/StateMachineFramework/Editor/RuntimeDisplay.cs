using StateMachineFramework.Runtime;
namespace StateMachineFramework.Editor {

    public class RuntimeDisplay {
        private SMWindow w;

        public RuntimeDisplay(SMWindow window) {
            this.w = window;

        }
        public void Init() {
            w.stateMachine.logic.OnNodeEnter += Highlight;
            w.stateMachine.logic.OnNodeExit += RemoveHighlight;
            w.depthPanel.OnDepthChanged += DepthChanged;
            DepthChanged(w.depthPanel.ActiveTree);
        }
        public void Clear() {
            w.stateMachine.logic.OnNodeEnter -= Highlight;
            w.stateMachine.logic.OnNodeExit -= RemoveHighlight;
            w.depthPanel.OnDepthChanged -= DepthChanged;

        }
        private void DepthChanged(TreeNode tree) {

            foreach (var a in w.stateMachine.logic.activeNodes) {
                if (w.depthPanel.IsInScope(a))
                    w.nodeView.nodes[a].AddToClassList("active");
            }
        }

        private void Highlight(Node node) {
            if (w.depthPanel.IsInScope(node))
                w.nodeView.nodes[node].AddToClassList("active");
        }
        private void RemoveHighlight(Node node) {
            if (w.depthPanel.IsInScope(node))
                w.nodeView.nodes[node].RemoveFromClassList("active");

        }

        public void Update() {
            //Debug.Log(w.stateMachine.logic.activeNodes.Count); 
        }
    }
}