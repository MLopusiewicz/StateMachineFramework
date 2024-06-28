using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineFramework.Runtime {

    [Serializable]
    public class TreeNode : Node, ISerializationCallbackReceiver {

        [SerializeReference]
        public List<Node> nodes = new();

        [NonSerialized]
        public Node enterNode, exitNode;

        [SerializeField]
        public Vector2 enterPos, exitPos, anyPos;

        public TreeNode() {
            enterNode = new SpecialNode() { name = "Enter", position = new Vector2(0, 200), canBeSource = true, canBeTarget = false };
            exitNode = new SpecialNode() { name = "Exit", position = new Vector2(500, 200), canBeSource = false, canBeTarget = true };
        }

        public void OnBeforeSerialize() {
            enterPos = enterNode.position;
            exitPos = exitNode.position;
        }


        public void OnAfterDeserialize() {
            enterNode.position = enterPos;
            exitNode.position = exitPos;
            foreach (var a in nodes) {
                a.parent = this;
            }
        }
        public override string ToString() {
            return $"Tree: {name}";
        }

    }
}