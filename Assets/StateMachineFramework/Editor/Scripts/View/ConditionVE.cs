using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static StateMachineFramework.Runtime.ParameterController;

namespace StateMachineFramework.View {
    public class ConditionVE : VisualElement {

        public const string CONDITION = "condition";
        public const string CONDITION_PARAM_BUTTON = "condition-param-button";
        public const string CONDITION_PARAM_SEARCH = "condition-param-search";
        public const string CONDITION_PARAM_TYPE = "condition-param";
        public const string CONDITION_PROPERTY = "condition-property";
        public const string CONDITION_ENUM = "condition-property__enum";
        public const string CONDITION_VALUE = "condition-property__value";


        enum boolType { True, False }
        public class ConditionsFactory : UxmlFactory<ConditionVE, ConditionTraits> { }
        public class ConditionTraits : UxmlTraits {
            public UxmlEnumAttributeDescription<ParameterType> paramType = new UxmlEnumAttributeDescription<ParameterType>() { name = "paramType", defaultValue = ParameterType.Float };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<ConditionVE>();
                var pp = paramType.GetValueFromBag(bag, cc);
            }
        }

        Button parameterButton;
        VisualElement propContainer;
        Action changeCallback;
        public ConditionVE() {
            SetupContainers();
        }

        public void SetupContainers() {
            this.AddToClassList("condition");
            parameterButton = VEHelper.Make<Button>(this, "ParameterButton", CONDITION_PARAM_BUTTON);
            propContainer = VEHelper.Make<VisualElement>(this, "PropertyContainer", CONDITION_PROPERTY);
            parameterButton.text = "parameter name";
            parameterButton.clicked += CallCallback;
        }

        private void CallCallback() {
            changeCallback?.Invoke();
        }

        internal void Init(UnityEditor.SerializedProperty condition, Action changeCallback) {
            propContainer.Clear();
            parameterButton.BindProperty(condition.FindPropertyRelative("parameter").FindPropertyRelative("key"));
            var c = condition.FindPropertyRelative("equation").GetEnumerator();
            bool first = true;
            while (c.MoveNext()) {
                var z = c.Current as SerializedProperty;
                var f = new PropertyField(z);
                if (first) {
                    f.AddToClassList(CONDITION_ENUM);
                    first = false;
                } else
                    f.AddToClassList(CONDITION_VALUE);
                f.Bind(z.serializedObject);
                propContainer.Add(f);
            }
            this.changeCallback = changeCallback;
        }

    }

}