using StateMachineFramework.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class NodeVE : VisualElement {

        public const string NODE = "node";
        public const string NODE_LABEL = "node-label";
        public const string NODE_NAME_INPUT = "node-name-field";
        public const string NODE_SPECIAL_ENTER = "node-special-enter";
        public const string NODE_SPECIAL_EXIT = "node-special-exit";
        public const string NODE_SPECIAL_ANY = "node-special-any";
        public const string NODE_SPECIAL_DEFAULT = "node-special-default";

        public class NodeFactory : UxmlFactory<NodeVE, NodeTraits> { }

        public class NodeTraits : UxmlTraits {
            UxmlIntAttributeDescription xPos = new UxmlIntAttributeDescription() { name = "X" };
            UxmlIntAttributeDescription yPos = new UxmlIntAttributeDescription() { name = "Y" };

            UxmlStringAttributeDescription nodeName = new UxmlStringAttributeDescription() { name = "displayName" };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<NodeVE>();
                target.pos = new Vector2(xPos.GetValueFromBag(bag, cc), yPos.GetValueFromBag(bag, cc));
                target.displayName = nodeName.GetValueFromBag(bag, cc);
                target.data = new Node() { name = target.displayName, position = target.pos };
                target.Init(target.data);
            }
        }

        public Vector2 pos { get; set; }
        public string displayName { get; set; }
        Label label;
        TextField textField;
        Node data;

        public NodeVE() {
            this.usageHints = UsageHints.DynamicTransform;
            SetupContainters();
            this.RegisterCallback<MouseDownEvent>(OnClicked);
            this.RegisterCallback<FocusOutEvent>(OnLostFocus);
        }

        private void OnLostFocus(FocusOutEvent evt) {
            label.SetDisplay(true);
            textField.SetDisplay(false);
        }

        private void OnClicked(MouseDownEvent evt) {
            if (evt.clickCount != 2)
                return;
            if (data is SpecialNode)
                return;
            RenameState();
        }
        public void RenameState() {

            label.SetDisplay(false);
            textField.SetDisplay(true);
            textField.Focus();
        }

        public void SetupContainters() {
            this.AddToClassList(NODE);
            label = new Label();
            label.AddToClassList(NODE_LABEL);
            label.pickingMode = PickingMode.Ignore;
            this.Add(label);
            textField = new TextField();
            this.Add(textField);
            textField.SetDisplay(false);
            textField.AddToClassList(NODE_NAME_INPUT);
        }


        public void Init(Node node) {
            data = node;
            pos = node.position;
            this.transform.position = node.position;
            label.text = node.name;
        }

        public void Init(Node node, SerializedProperty _node) {
            Init(node);
            if (_node == null)
                return;
            label.BindProperty(_node.FindPropertyRelative("name"));
            textField.BindProperty(_node.FindPropertyRelative("name"));
        }

        internal void SetDefault(bool v) {
            this.EnableInClassList(NODE_SPECIAL_DEFAULT, v);
        }
    }
}