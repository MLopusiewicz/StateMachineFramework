using System;
using System.Collections.Generic;
using UnityEngine;
namespace StateMachineFramework.Runtime {

    public class StateMachine : MonoBehaviour {

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

        public TreeNode RootTree => (TreeNode)_nodes[0];
        public Node AnyState => _nodes[1];
        public List<Node> Nodes => _nodes;
        public List<IParameter> GetAllParameters => _parameters;


        bool UpdateInterruptToken;
        private void Awake() {
            logic = new StateMachineLogic(_nodes, _parameters);
            parameters = new ParameterController(_parameters);
            logic.OnNodeExit += InterruptUpdate;
            foreach (var node in _nodes) {
                foreach (var behaviour in node.behaviours) {
                    behaviour.Awake(this.gameObject);
                }
            }
        }

        private void Start() {
            foreach (var node in _nodes) {
                foreach (var behaviour in node.behaviours) {
                    behaviour.Start();
                }
            }

            logic.Start();
        }

        private void Update() {
            UpdateInterruptToken = false;
            foreach (var node in logic.activeNodes) {
                foreach (var behaviour in node.behaviours) {
                    if (UpdateInterruptToken)
                        break;
                    behaviour.Update();
                }
                if (UpdateInterruptToken)
                    break;
            }

        }
        private void LateUpdate() {
            UpdateInterruptToken = false;
            foreach (var node in logic.activeNodes) {
                foreach (var behaviour in node.behaviours) {
                    if (UpdateInterruptToken)
                        break;
                    behaviour.LateUpdate();
                }
                if (UpdateInterruptToken)
                    break;
            }

        }

        private void InterruptUpdate(Node node) {
            UpdateInterruptToken = true;
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