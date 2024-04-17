using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace StateMachineFramework.Runtime {
    public class StateMachineLogic {

        public event Action<Node> OnNodeEnter, OnNodeExit;
        public List<Node> activeNodes = new();
        public List<Node> nodes;

        int transitionCount = 0;
        int transitionsPerFrameTreshhold = 30;
        Transition overflownTransition;
        public StateMachineLogic(List<Node> nodes, List<IParameter> parameters) {
            this.nodes = nodes;
            foreach (var a in nodes) {
                a.OnEntered += NodeEntered;
                a.OnExited += NodeExited;
            }
            foreach (var a in parameters) {
                a.OnValueChanged += CheckTransitions;
            }
        }

        private void CheckTransitions() {
            transitionCount = 0;
            foreach (var transition in nodes[1].transitions) {
                if (transition.conditions.Count == 0)
                    continue;

                if (transition.Evaluate()) {
                    Move(transition);
                    return;
                }
            }
            foreach (var node in activeNodes) {
                foreach (var transition in node.transitions) {
                    if (transition.target.parent == transition.source)
                        continue;
                    if (transition.Evaluate()) {
                        Move(transition);
                        return;
                    }
                }
            }
        }

        public void Move(Transition t) {
            if (t == null)
                return;

            if (transitionCount > transitionsPerFrameTreshhold) {
                Debug.LogError("[SMF] Transition treshhold per fame exceeded. Moving execution to new frame");
                overflownTransition = t;
                return;
            }

            transitionCount++;
            if (t.source == nodes[1])
                t.source = activeNodes[^1];

            bool reentry = activeNodes.Contains(t.target);

            Node commonAncestor = t.source;
            if (t.source == null)
                commonAncestor = this.activeNodes[^1];
            var targetAncestors = SMHelper.GetAncestors(t.target);

            while (!targetAncestors.Contains(commonAncestor)) {
                commonAncestor.Exit();
                commonAncestor = commonAncestor.parent;
            }

            int index = targetAncestors.IndexOf(commonAncestor);
            for (int i = index - 1; i >= 0; i--) {
                targetAncestors[i].Enter();
            }

            if (reentry) {
                t.target.Exit();
                t.target.Enter();
            }

            //Reset triggers 
            foreach (var a in t.conditions) {
                if (a.parameter is TriggerParameter tp) {
                    tp.SetValue(false);
                }
            }
        }

        private void NodeExited(Node node) {
            Debug.Log($"[SM] Exited: {node.name}");
            activeNodes.Remove(node);
            OnNodeExit?.Invoke(node);
        }

        private void NodeEntered(Node node) {
            Debug.Log($"[SM] Entered: {node.name}");
            activeNodes.Add(node);
            var t = GetFirstValid(node);
            if (t != null)
                Move(t);
            else if (node is TreeNode tree) {

            }
            OnNodeEnter?.Invoke(node);
        }

        public Transition GetFirstValid(Node n) {
            foreach (var a in n.transitions) {
                if (a.Evaluate())
                    return a;
            }
            return null;
        }

        internal void Start() {
            nodes[0].Enter();
        }


        public void EnterState(string state) {
            transitionCount = 0;
            foreach (var a in nodes) {
                if (a.name == state) {
                    Move(new Transition() { source = nodes[1], target = a });
                    return;
                }
            }
        }
        public void Update() {
            if (overflownTransition != null) {
                transitionCount = 0;
                var g = overflownTransition;
                overflownTransition = null;
                Move(g);
            }
        }
    }
}