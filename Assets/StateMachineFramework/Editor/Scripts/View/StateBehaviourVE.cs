using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class StateBehaviourVE : VisualElement {

        public const string BEHAVIOUR = "behaviour";
        public const string BEHAVIOUR_HEADER = "behaviour-header";
        public const string BEHAVIOUR_HEADER_BUTTONS_CONTAINER = "behaviour-header-buttons-container";
        public const string BEHAVIOUR_PROP_CONTAINER = "behaviour-property-container";
        public const string BEHAVIOUR_SCRIPT_BUTTON = "behaviour-script-button";
        public const string BEHAVIOUR_HEADER_CONTAINER = "behaviour-header-container";
        public const string BEHAVIOUR_PROP_REMOVE = "behaviour-remove-button";
        public const string BEHAVIOUR_DROPDOWN_ARROR = "behaviour-dropdown-arrow";
        public class StateBehaviourFactory : UxmlFactory<StateBehaviourVE, StateBehaviourTraits> { }
        public class StateBehaviourTraits : UxmlTraits {
            public UxmlStringAttributeDescription scriptName = new UxmlStringAttributeDescription() { name = "ScriptName" };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<StateBehaviourVE>();
                target.ScriptName = scriptName.GetValueFromBag(bag, cc);
                target.header.text = target.ScriptName;
                target.SetupScriptButton(target.ScriptName);
            }
        }

        Label header;
        VisualElement propertyContainer;
        VisualElement scriptButton;
        VisualElement headerContainer;
        VisualElement dropdownArrow;
        public string ScriptName { get; set; }
        string path;
        bool displayState = true;

        public StateBehaviourVE() {
            SetupContainers();
            headerContainer.RegisterCallback<MouseDownEvent>(HeaderClicked);
            Toggle(true);
        }

        private void HeaderClicked(MouseDownEvent evt) {
            Toggle(displayState ^ true);
        }
        void Toggle(bool state) {

            propertyContainer.SetDisplay(state);
            dropdownArrow.EnableInClassList("toggled", state);
            displayState = state;
        }
        public void SetupContainers() {
            this.AddToClassList(BEHAVIOUR);
            headerContainer = VEHelper.Make<VisualElement>(this, "HeaderContainer", BEHAVIOUR_HEADER_CONTAINER);
            propertyContainer = VEHelper.Make<VisualElement>(this, "PropertyContainer", BEHAVIOUR_PROP_CONTAINER);

            dropdownArrow = VEHelper.Make<VisualElement>(headerContainer, "DropdownArrow", BEHAVIOUR_DROPDOWN_ARROR);
            header = VEHelper.Make<Label>(headerContainer, "Header", BEHAVIOUR_HEADER);

            scriptButton = VEHelper.Make<VisualElement>(headerContainer, "OpenScriptButon", BEHAVIOUR_SCRIPT_BUTTON);

        }

        public void Init(SerializedProperty prop) {
            var enumerator = prop.GetEnumerator();
            propertyContainer.Clear();

            var t = prop.managedReferenceValue.GetType();
            SetupScriptButton(t.Name);
            int depth = prop.depth + 1;
            while (enumerator.MoveNext()) {
                var z = enumerator.Current as SerializedProperty;
                if (z.depth > depth)
                    continue;
                var f = new PropertyField(z);
                f.Bind(z.serializedObject);
                propertyContainer.Add(f);
                 
            }
        }

        void SetupScriptButton(string typeName) {

            header.text = typeName;
            var files = AssetDatabase.FindAssets($"t:script {typeName}");
            scriptButton.SetEnabled(files.Length > 0);

            if (files.Length > 0) {
                path = files[0];
                scriptButton.RegisterCallback<MouseDownEvent>(ScriptPressed);
            }
        }

        private void ScriptPressed(MouseDownEvent evt) {
            if (evt.clickCount == 1) {
                var z = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(path));
                EditorGUIUtility.PingObject(z);
                //if (z != null)
                //    UnityEditor.Selection.activeObject = z;
                evt.StopImmediatePropagation();
            }

        }

    }
}