using StateMachineFramework.Runtime;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class ParameterVE : VisualElement {
        public const string PARAMETER_NAME_FIELD = "parameter-name-field";
        public const string PARAMETER = "parameter";
        public const string PARAMETER_VALUE = "parameter-value";

        TextField nameField;



        public Action<VisualElement, IParameter> removeRequested;
        VisualElement valueProp;

        public Action ValueChanged;
        public ParameterVE() {
            SetupContainers();
        }

        protected virtual void SetupContainers() {
            this.AddToClassList("parameter");
            nameField = VEHelper.Make<TextField>(this, "Name", PARAMETER_NAME_FIELD);
        }


        public virtual void Init(SerializedProperty pp) {
            nameField.BindProperty(pp.FindPropertyRelative("key"));
            if (valueProp != null)
                valueProp.RemoveFromHierarchy();

            IBindable ve = null;
            if (pp.managedReferenceValue is TriggerParameter) {
                ve = VEHelper.Make<Toggle>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__trigger");
            }
            if (pp.managedReferenceValue is BoolParameter) {
                ve = VEHelper.Make<Toggle>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__bool");
            }
            if (pp.managedReferenceValue is IntParameter) {
                ve = VEHelper.Make<IntegerField>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__int");
            }
            if (pp.managedReferenceValue is FloatParameter) {
                ve = VEHelper.Make<FloatField>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__float");
            }
            ve.BindProperty(pp.FindPropertyRelative("value"));
            valueProp = ve as VisualElement;
        }
        public void InitRuntime(IParameter pp) {
            nameField.value = pp.Key;
            if (valueProp != null)
                valueProp.RemoveFromHierarchy();


            if (pp is TriggerParameter t) {
                var ve = VEHelper.Make<Toggle>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__trigger");
                ve.value = t.Value;
                ve.RegisterCallback<ChangeEvent<bool>>(x => t.SetValue(x.newValue));
                t.OnChanged += ve.SetValueWithoutNotify;
                valueProp = ve;
            }
            if (pp is BoolParameter b) {
                var ve = VEHelper.Make<Toggle>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__bool");
                ve.value = b.Value;
                ve.RegisterCallback<ChangeEvent<bool>>(x => b.SetValue(x.newValue));
                b.OnChanged += ve.SetValueWithoutNotify;
                valueProp = ve;
            }
            if (pp is IntParameter i) {
                var ve = VEHelper.Make<IntegerField>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__int");
                ve.value = i.Value;
                ve.RegisterCallback<ChangeEvent<int>>(x => i.SetValue(x.newValue));
                i.OnChanged += ve.SetValueWithoutNotify;
            }
            if (pp is FloatParameter f) {
                var ve = VEHelper.Make<FloatField>(this, "Prop", PARAMETER_VALUE, $"{PARAMETER_VALUE}__float");
                ve.value = f.Value;
                ve.RegisterCallback<ChangeEvent<float>>(x => f.SetValue(x.newValue));
                f.OnChanged += ve.SetValueWithoutNotify;
                valueProp = ve;
            }
        }

    }
}