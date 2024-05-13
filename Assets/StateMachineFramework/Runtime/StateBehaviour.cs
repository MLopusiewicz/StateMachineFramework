using System;

namespace StateMachineFramework.Runtime {

    [Serializable]
    public abstract class StateBehaviour {

        public virtual void Awake() { }
        public virtual void Update() { }
        public abstract void Enter();
        public abstract void Exit();
        public virtual void OnDestroy() { }
    }
}
