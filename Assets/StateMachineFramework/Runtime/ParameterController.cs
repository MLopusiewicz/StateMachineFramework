using System;
using System.Collections.Generic;
using UnityEngine;
namespace StateMachineFramework.Runtime {

    public class ParameterController {
        public enum ParameterType { Trigger = 2, Bool = 4, Int = 8, Float = 16 };


        Dictionary<ParameterType, Dictionary<string, IParameter>> LUT;

        Dictionary<Type, ParameterType> TypeEnumParser = new() {
        {typeof(TriggerParameter), ParameterType.Trigger},
        {typeof(FloatParameter), ParameterType.Float},
        {typeof(IntParameter), ParameterType.Int},
        {typeof(BoolParameter), ParameterType.Bool},
    }; 
        public ParameterController(List<IParameter> parameters) { 

            LUT = new();
            foreach (var paramType in (ParameterType[])Enum.GetValues(typeof(ParameterType))) {
                LUT.Add(paramType, new Dictionary<string, IParameter>());
            }
            foreach (var param in parameters) {
                LUT[TypeEnumParser[param.GetType()]].Add(param.Key, param);
            }
        }

        public void SetTrigger(string name) {
            SetValue(ParameterType.Trigger, name, true);
        }
        public void FlashTrigger(string name) {
            SetValue(ParameterType.Trigger, name, true);
            SetValue(ParameterType.Trigger, name, false);
        }

        public void ResetTrigger(string name) {
            SetValue(ParameterType.Trigger, name, false);
        }

        public void SetBool(string name, bool value) {
            SetValue(ParameterType.Bool, name, value);
        }

        public void SetFloat(string name, float value) {
            SetValue(ParameterType.Float, name, value);
        }
        public void SetInt(string name, int value) {
            SetValue(ParameterType.Int, name, value);
        }


        public bool GetTrigger(string name) {
            return GetValue<bool>(ParameterType.Trigger, name);
        }
        public bool GetBool(string name) {
            return GetValue<bool>(ParameterType.Bool, name);
        }
        public float GetFloat(string name) {
            return GetValue<float>(ParameterType.Float, name);
        }
        public int GetInt(string name) {
            return GetValue<int>(ParameterType.Int, name);
        }

        void SetValue<T>(ParameterType type, string name, T value) {
            if (!LUT[type].ContainsKey(name)) {
                Debug.LogWarning($"[SM] {type} not found: {name}");
                return;
            }
            ((Parameter<T>)LUT[type][name]).Value = value;
        }
        T GetValue<T>(ParameterType type, string name) {
            if (!LUT[type].ContainsKey(name)) {
                Debug.LogWarning($"[SM] Parameter not found: {name}({type})");
                return default;
            }
            return ((Parameter<T>)LUT[type][name]).Value;
        }



    }
}