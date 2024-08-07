﻿using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

namespace StateMachineFramework.Runtime {

    [Serializable]
    public class Transition {
        [SerializeReference]
        public Node source, target;

        public List<TransitionCondition> conditions = new();
        public bool Evaluate() {
            foreach (var condition in conditions) {
                if (condition.Evaluate() == false)
                    return false;
            }
            return true;
        }
        public override string ToString() {
            return $"{source.name} -> {target.name}";
        }
    }
}