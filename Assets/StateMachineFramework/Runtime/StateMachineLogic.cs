using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace StateMachineFramework.Runtime {
    public class StateMachineLogic {

        public event Action<Node> OnNodeEnter, OnNodeExit;
        public List<Node> activeNodes = new();
        public List<Node> nodes;

        public Node AnyStateNode => nodes[1];
        public TreeNode RootTree => (TreeNode)nodes[0];

        int transitionCount = 0;
        int transitionsPerFrameTreshhold = 30;
        Transition overflownTransition;

        bool transitionsBlocked = false;

        public StateMachineLogic(List<Node> nodes, List<IParameter> parameters) {
            this.nodes = nodes;
            foreach (var a in parameters) {
                a.OnValueChanged += CheckTransitions;
            }

        }

        public void CheckTransitions() {
            if (transitionsBlocked)
                return;

            transitionCount = 0;
            foreach (var transition in AnyStateNode.transitions) {
                if (transition.conditions.Count == 0)
                    continue;

                if (transition.Evaluate()) {
                    Move(transition);
                    return;
                }
            }
            for (int i = activeNodes.Count - 1; i >= 0; i++) {
                var node = activeNodes[i];
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

        void Move(Transition t) {
            if (t == null)
                return;

            transitionsBlocked = true;
            //Reset triggers 
            foreach (var a in t.conditions) {
                if (a.parameter is TriggerParameter tp) {
                    tp.SetValue(false);
                }
            }

            transitionCount++;
            if (transitionCount > transitionsPerFrameTreshhold) {
                Debug.LogError("[SMF] Transition treshhold per fame exceeded. Moving execution to new frame");
                overflownTransition = t;
                transitionsBlocked = false;
                return;
            }

            if (t.source == AnyStateNode)
                t.source = activeNodes[^1];

            var activeIndex = activeNodes.IndexOf(t.source);

            //Exit all children  Bottoms up /\ 
            for (int i = activeNodes.Count - 1; i >= activeIndex + 1; i--)
                ExitNode(activeNodes[i]);



            bool reentry = activeNodes.Contains(t.target);

            Node commonAncestor = t.source;
            if (t.source == null)
                commonAncestor = this.activeNodes[^1];
            var targetAncestors = SMHelper.GetAncestors(t.target);

            while (!targetAncestors.Contains(commonAncestor)) {
                ExitNode(commonAncestor);
                commonAncestor = commonAncestor.parent;
            }

            int index = targetAncestors.IndexOf(commonAncestor);
            for (int i = index - 1; i >= 0; i--) {
                EnterNode(targetAncestors[i]);
            }

            if (reentry) {
                ExitNode(t.target);
                EnterNode(t.target);
            }

            transitionsBlocked = false;
            CheckTransitions();

        }


        void EnterNode(Node node) {
            node.Enter();
            OnNodeEnter?.Invoke(node);

            activeNodes.Add(node);
            var t = GetFirstValid(node);
            if (t != null)
                Move(t);
            else if (node is TreeNode tree) {

            }

        }
        void ExitNode(Node node) {
            node.Exit();
            activeNodes.Remove(node);
            OnNodeExit?.Invoke(node);

        }




        public Transition GetFirstValid(Node n) {
            foreach (var a in n.transitions) {
                if (a.Evaluate())
                    return a;
            }
            return null;
        }

        internal void Start() {
            EnterNode(RootTree);
        }

        public void EnterState(string state) {
            transitionCount = 0;
            foreach (var a in nodes) {
                if (a.name == state) {
                    Move(new Transition() { source = AnyStateNode, target = a });
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