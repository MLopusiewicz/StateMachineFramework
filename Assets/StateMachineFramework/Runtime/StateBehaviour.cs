using System;
using UnityEngine;

namespace StateMachineFramework.Runtime {

    [Serializable]
    public abstract class StateBehaviour {
        public virtual void Awake(GameObject obj) { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public abstract void Enter();
        public abstract void Exit();
        public virtual void OnDestroy() { }
    }

}
