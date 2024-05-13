using System;
using System.Collections.Generic;
using UnityEngine;
namespace StateMachineFramework.Runtime {

    public class StateMachineFramework : MonoBehaviour {

        public ParameterController parameters;
        public StateMachineLogic logic;

        [SerializeReference]
        List<Node> _nodes = new() {
        new TreeNode() { name = "Root" },
        new SpecialNode() { name = "Any State",
            canBeSource = true,
            canBeTarget = false }
    };

        [SerializeReference]
        List<IParameter> _parameters = new();

        public List<Node> Nodes => _nodes;
        public List<IParameter> Parameters => _parameters;

        public TreeNode Root => (TreeNode)_nodes[0];
        public SpecialNode anyState => (SpecialNode)Nodes[1];

        private void Awake() {
            logic = new StateMachineLogic(_nodes, _parameters);
            parameters = new ParameterController(_parameters);

            foreach (var node in _nodes) {
                foreach (var behaviour in node.behaviours) {
                    behaviour.Awake();
                }
            }
        }
        private void Update() {
            foreach (var node in logic.activeNodes) {
                foreach (var behaviour in node.behaviours) {
                    behaviour.Update();
                }
            }
        }

        private void Start() {
            logic.Start();
        }
        private void OnDestroy() {
            foreach (var node in logic.nodes) {
                foreach (var behaviour in node.behaviours) {
                    behaviour.OnDestroy();
                }

            }
        }
    }

    public static class SMHelper {
        public static Node FindCommonAncestor(Node a, Node b) {
            var ancestorsA = GetAncestors(a);
            var ancestorsB = GetAncestors(b);
            for (int i = 0; i < ancestorsB.Count; i++) {
                if (ancestorsA.Contains(ancestorsB[i]))
                    return ancestorsB[i];
            }
            throw new Exception("This should never happen");
        }
        public static List<Node> GetAncestors(Node a) {
            List<Node> list = new List<Node> { a };
            while (list[^1].parent != null) {
                list.Add(list[^1].parent);
            }
            return list;
        }

    }
}