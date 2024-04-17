using System;
using UnityEngine;
using UnityEngine.UIElements;
using static StateMachineFramework.Runtime.ParameterController;


namespace StateMachineFramework.Runtime {


    [Serializable]
    public abstract class Parameter<T> : IDisposable, IParameter {
        [SerializeField]
        string key;
        public string Key => key;
        public event Action<string> OnKeyChanged;
        public event Action OnValueChanged;
        public ParameterType paramType { get; set; }
        public Parameter() {
            key = "new parameter";
        }

        public Parameter(string key, T value) {
            this.key = key;
            this.value = value;
        }

        public T Value {
            get { return this.value; }
            set {
                if (this.value != null) {
                    if (!this.value.Equals(value)) {
                        this.value = value;
                        OnChanged?.Invoke(value);
                        OnValueChanged?.Invoke();
                    }
                } else if (value != null) {
                    this.value = value;
                    OnChanged?.Invoke(value);
                    OnValueChanged?.Invoke();
                }
            }
        }

        public event Action<T> OnChanged;

        [SerializeField]
        T value;


        public void SetNoNotify(T v) {
            value = v;
        }



        public void SubscribeAndCall(Action<T> action) {
            OnChanged += action;
            action?.Invoke(value);
        }
         
        public void SetValue(T v) {
            this.Value = v;
        }

        public void Dispose() {
            OnChanged = null;
        }

        public override string ToString() {
            if (value != null)
                return $"Parameter<{typeof(T).Name}>: {value}";
            else
                return $"Parameter<{typeof(T).Name}>: NULL";
        }

        public static implicit operator T(Parameter<T> d) => d.Value;

        public void UpdateKey(string newKey) {
            this.key = newKey;
            OnKeyChanged?.Invoke(newKey);
        }

    }


    public class TriggerParameter : Parameter<bool> {
        public TriggerParameter(string name, bool value) : base(name, value) { }
    }
    public class BoolParameter : Parameter<bool> {
        public BoolParameter(string name, bool value) : base(name, value) { }
    }
    public class FloatParameter : Parameter<float> {
        public FloatParameter(string name, float value) : base(name, value) { }
    }
    public class IntParameter : Parameter<int> {
        public IntParameter(string name, int value) : base(name, value) { }
    }

    public interface IParameter {
        public string Key { get; }

        event Action<string> OnKeyChanged;
        event Action OnValueChanged;
    }
}