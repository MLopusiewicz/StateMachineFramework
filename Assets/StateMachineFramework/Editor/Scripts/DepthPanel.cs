using StateMachineFramework.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class DepthPanel {

        public Action<TreeNode> OnDepthChanged;
        public TreeNode ActiveTree => depth[^1];

        StateMachineEditor w;
        List<TreeNode> depth = new();
        //List<SerializedProperty> depth = new List<SerializedProperty>();
        Dictionary<TreeNode, VisualElement> nodeButtons = new();
        VisualElement container;
        VisualElement depthContainer;

        public DepthPanel(StateMachineEditor w) {
            this.w = w;
            container = w.rootVisualElement.Q(name: "DepthPanel");
            depthContainer = container.Q(name: "DepthContainer");
            depthContainer.Clear();
            w.serialization.OnNodeAdded += AddNodeToActive;
            w.serialization.OnNodeRemoved += RemoveNodeFromActive;
        }

        public void Init() {
            depth.Clear();
            depthContainer.Clear();
            nodeButtons.Clear();
            AddDepthLayer(w.stateMachine.RootTree, false);
        }

        private void AddNodeToActive(Node node) {
            ActiveTree.nodes.Add(node);
        }

        private void RemoveNodeFromActive(Node node) {
            ActiveTree.nodes.Remove(node);
        }

        public void AddDepthLayer(TreeNode node, bool notify = true) {
            if (depth.Contains(node))
                return;
            var ve = new Label();
            ve.text = node.name;
            ve.AddToClassList("depth-node-label");
            ve.AddManipulator(new Clickable(() => TreeSelected(node)));
            depth.Add(node);
            depthContainer.Add(ve);
            ve.SendToBack();
            nodeButtons.Add(node, ve);
            if (notify)
                OnDepthChanged?.Invoke(node);
        }

        void TreeSelected(TreeNode node) {
            var index = depth.IndexOf(node) + 1;
            var removal = depth.GetRange(index, depth.Count - index);
            foreach (var a in removal) {
                nodeButtons[a].RemoveFromHierarchy();
                nodeButtons.Remove(a);
            }

            depth.RemoveRange(index, depth.Count - index);

            OnDepthChanged?.Invoke(node);
        }

        public bool IsInScope(Node n) {
            return this.ActiveTree.nodes.Contains(n);
        }
    }
}