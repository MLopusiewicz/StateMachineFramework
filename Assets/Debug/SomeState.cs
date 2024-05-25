using System;
using UnityEngine;

namespace StateMachineFramework.Runtime {
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
