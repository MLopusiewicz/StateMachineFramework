using System;
using UnityEngine;

namespace StateMachineFramework.Runtime {

    [Serializable]
    public abstract class StateBehaviour {

        public virtual void Awake() { }
        public virtual void Update() { }
        public abstract void Enter();
        public abstract void Exit();

    }

    [Serializable]
    public class SomeState : StateBehaviour {

        [SerializeField]
        public GameObject referenceToSomething;
        public int amoutn;

        public override void Enter() {
            Debug.Log($"Entered State");
        }
        public override void Exit() {
            Debug.Log($"Exited State");
        }
    }
}
