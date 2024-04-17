using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class NodeInspector {

        TextField nameField;
        Node selectedNode;
        Window w;
        SearchPopupVE searchPopup;
        VisualElement container;
        ListView behaviourList;
        SerializedProperty serializedBehaviours;
        Button addButton;
        Dictionary<string, Type> typeLut;
        public NodeInspector(Window w) {
            container = w.rootVisualElement.Q(name: "NodeInspector");
            container.SetDisplay(false);
            nameField = container.Q<TextField>(); 
            this.w = w;

            searchPopup = container.Q<SearchPopupVE>();
            searchPopup.OnEntrySelected += AddBehaviourOfType;
            searchPopup.Init(GetTypes());
            searchPopup.Hide();

            ListSetup();
        }
        void ListSetup() {
            behaviourList = container.Q<ListView>(name: "BehaviourList");

            behaviourList.makeItem = () => {
                return new StateBehaviourVE();
            };

            behaviourList.bindItem = (ve, index) => {
                var v = ve.Q<StateBehaviourVE>();
                var p = serializedBehaviours.GetArrayElementAtIndex(index);
                v.Init(p);
            };

            behaviourList.itemIndexChanged += OnReordered;
            behaviourList.itemsRemoved += Removed;

            behaviourList.Q<Button>(name: "unity-list-view__add-button").SetDisplay(false);

            addButton = new Button();
            addButton.AddToClassList("parameter-add-dropdown");
            behaviourList.Q(name: "unity-list-view__footer").Add(addButton);
            addButton.text = "+";
            addButton.clicked += searchPopup.Show;
        }

        private void Removed(IEnumerable<int> enumerable) {
            List<int> indexes = new(enumerable);
            indexes.Reverse();
            foreach (var i in indexes) {
                serializedBehaviours.DeleteArrayElementAtIndex(i);
            }

            w.serialization.Apply();
            RefreshList();
        }


        void OnReordered(int arg1, int arg2) {
            serializedBehaviours.MoveArrayElement(arg1, arg2);
            w.serialization.Apply();
            RefreshList();
        }
        void RefreshList() {
            behaviourList.itemsSource = selectedNode.behaviours;
            behaviourList.RefreshItems();
        }


        public void Clear() {
            behaviourList.itemsSource = null;
            behaviourList.Clear();
            return;
        }

        public void Show(Node node) {
            if (node == null) {
                Clear();
                return;
            }

            if (node is SpecialNode) {
                behaviourList.itemsSource = null;
                return;
            }
            selectedNode = node;
            var serNode = w.serialization.GetSerializedNode(selectedNode);
            serializedBehaviours = serNode.FindPropertyRelative("behaviours");
            behaviourList.itemsSource = node.behaviours;
            container.SetDisplay(true);
            nameField.BindProperty(serNode.FindPropertyRelative("name"));
        }

        public void AddBehaviourOfType(string s) {
            serializedBehaviours.arraySize++;

            serializedBehaviours.GetArrayElementAtIndex(serializedBehaviours.arraySize - 1).managedReferenceValue = Activator.CreateInstance(typeLut[s]);
            serializedBehaviours.serializedObject.ApplyModifiedProperties();
            RefreshList();
        }

        List<string> GetTypes() {
            typeLut = new();
            var listOfBs = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(domainAssembly => domainAssembly.GetTypes())
                 .Where(type => typeof(StateBehaviour).IsAssignableFrom(type)
                            && !type.IsAbstract)
                 .ToList();

            foreach (var tt in listOfBs) {
                typeLut.Add(tt.Name, tt);
            }

            return listOfBs.Select(x => x.Name).ToList();
        }
    }
}