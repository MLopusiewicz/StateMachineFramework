using System;
using System.Collections.Generic;
using UnityEngine;
namespace StateMachineFramework.Runtime {

    [Serializable]
    public class Node {

        public string name;
        public Vector2 position;

        [SerializeReference]
        public List<StateBehaviour> behaviours = new();

        [SerializeReference]
        List<Transition> _transitions = new();

        public List<Transition> transitions => _transitions;

        public event Action<Node> OnEntered, OnExited;

        [NonSerialized]
        public Node parent;

        public Node() {

        }

        public virtual void Enter() {
            foreach (var a in behaviours) {
                a.Enter();
            }
            OnEntered?.Invoke(this);
        }

        public virtual void Exit() {
            foreach (var a in behaviours) {
                a.Exit();
            }
            OnExited?.Invoke(this);
        }
        public override string ToString() {
            return $"Node: {name}";
        }

    }
}