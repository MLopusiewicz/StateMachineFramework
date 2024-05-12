using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class NodeInspector {

        TextField nameField;
        StateMachineEditor editor;
        SearchPopupVE searchPopup;
        VisualElement container;
        ListView behaviourList;
        SerializedProperty serializedBehaviours;
        Button addButton;
        Dictionary<string, Type> typeLut;


        Node selectedNode;
        public NodeInspector(StateMachineEditor editor) {
            container = editor.rootVisualElement.Q(name: "NodeInspector");
            container.SetDisplay(false);
            nameField = container.Q<TextField>();
            this.editor = editor;

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
                try {
                    var p = serializedBehaviours.GetArrayElementAtIndex(index);
                    v.Init(p);
                } catch {
                    Debug.Log("asdf: " + index);
                }
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
                editor.serialization.Apply();
            }

            //RefreshList();
        }


        void OnReordered(int arg1, int arg2) {
            serializedBehaviours.MoveArrayElement(arg1, arg2);
            editor.serialization.Apply();
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
            editor.inspector.SetActive(container);
            selectedNode = node;
            var serNode = editor.serialization.GetSerializedNode(selectedNode);
            if (serNode == null)
                return;

            serializedBehaviours = serNode.FindPropertyRelative("behaviours");
            List<SerializedProperty> prop = new();
            for (int i = 0; i < serializedBehaviours.arraySize; i++) {
                prop.Add(serializedBehaviours.GetArrayElementAtIndex(i));
            }

            behaviourList.itemsSource = prop;
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

        public void Redraw() {
            Show(selectedNode);
        }
    }
}