using System;
using UnityEngine;
namespace StateMachineFramework.Runtime {

    [Serializable]
    public class DebugState : StateBehaviour {

        [SerializeField]
        public GameObject referenceToSomething;
        public bool cos;
        public override void Enter() {
        }
        public override void Exit() {
        }
    }
}
