using System;
using UnityEngine;

namespace StateMachineFramework.Runtime {
    [Serializable]
    public class TransitionCondition {
        [SerializeReference]
        public IParameter parameter;
        [SerializeReference]
        public Equation equation;
        public bool Evaluate() {
            return equation.Evaluate(parameter);
        }

    }
}