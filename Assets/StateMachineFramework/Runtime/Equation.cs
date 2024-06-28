using System;
namespace StateMachineFramework.Runtime {

    [Serializable]
    public abstract class Equation {
        public abstract bool Evaluate(IParameter parameter);
        public enum EquationType { greater, greaterOrEqual, Equal, NotEqual, lesser, lesserOrEqual };
    }

    [Serializable]
    public class TriggerEquation : Equation {
        public override bool Evaluate(IParameter parameter) {
            return ((TriggerParameter)parameter).Value;
        }
        public override string ToString() {
            return "Triggered";
        }
    }

    [Serializable]
    public class IntEquation : Equation {
        public EquationType type;
        public int value;

        public override bool Evaluate(IParameter parameter) {

            IntParameter i = (IntParameter)parameter;
            switch (type) {
                case EquationType.Equal:
                    return i == value;
                case EquationType.NotEqual:
                    return i != value;

                case EquationType.lesserOrEqual:
                    return i <= value;
                case EquationType.greaterOrEqual:
                    return i <= value;

                case EquationType.greater:
                    return i > value;
                case EquationType.lesser:
                    return i < value;
            }
            return false;
        }
        public override string ToString() {
            return $"? {type}: {value}";
        }
    }

    [Serializable]
    public class FloatEquation : Equation {
        public EquationType type;
        public float value;

        public override bool Evaluate(IParameter parameter) {
            FloatParameter i = (FloatParameter)parameter;
            switch (type) {
                case EquationType.Equal:
                    return i == value;
                case EquationType.NotEqual:
                    return i != value;

                case EquationType.lesserOrEqual:
                    return i <= value;
                case EquationType.greaterOrEqual:
                    return i <= value;

                case EquationType.greater:
                    return i > value;
                case EquationType.lesser:
                    return i < value;
            }
            return false;
        }

        public override string ToString() {
            return $"? {type}: {value}";
        }
    }

    [Serializable]
    public class BoolEquation : Equation {
        public bool value;

        public override bool Evaluate(IParameter parameter) {
            return ((BoolParameter)parameter).Value == value;
        }

        public override string ToString() {
            return $"? is: {value}";
        }
    }
}