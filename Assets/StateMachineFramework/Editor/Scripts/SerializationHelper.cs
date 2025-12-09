using StateMachineFramework.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TreeNode = StateMachineFramework.Runtime.TreeNode;

namespace StateMachineFramework.Editor {
    public class SerializationHelper {
        SerializedObject serializedObj;
        Runtime.StateMachine sm;

        public Action<Node> OnNodeAdded, OnNodeRemoved;


        public SerializationHelper() {
        }
        public void Init(Runtime.StateMachine sm) {
            this.sm = sm;
            serializedObj = new SerializedObject(sm);
        }

        public SerializedProperty GetSerializedNode(Node n) {
            var serializedNodes = serializedObj.FindProperty("_nodes");
            for (int i = 0; i < serializedNodes.arraySize; i++) {
                var serNode = serializedNodes.GetArrayElementAtIndex(i);
                if (serNode.managedReferenceValue == n)
                    return serNode;
            }
            return null;
        }
        public int GetNodeSerializedIndex(Node n) {
            var serializedNodes = serializedObj.FindProperty("_nodes");
            for (int i = 0; i < serializedNodes.arraySize; i++) {
                var serNode = serializedNodes.GetArrayElementAtIndex(i);
                if (serNode.managedReferenceValue == n)
                    return i;
            }
            return -1;
        }

        internal List<SerializedProperty> GetConditions(Transition a) {
            List<SerializedProperty> props = new();

            var table = ConditionsList(a);
            for (int j = 0; j < table.arraySize; j++) {
                props.Add(table.GetArrayElementAtIndex(j));
            }

            return props;
        }

        public SerializedProperty ConditionsList(Transition a) {
            var transitions = serializedObj.FindProperty("_transitions");
            return GetTransition(a).FindPropertyRelative("conditions");
        }

        public List<SerializedProperty> TranstionConditions(Transition a) {
            List<SerializedProperty> vals = new List<SerializedProperty>();
            var table = GetTransition(a).FindPropertyRelative("conditions");
            for (int i = 0; i < table.arraySize; i++) {
                vals.Add(table.GetArrayElementAtIndex(i));
            }
            return vals;
        }

        public void AddNode(Node n) {
            var serializedNodes = serializedObj.FindProperty("_nodes");
            serializedNodes.arraySize++;
            serializedNodes.GetArrayElementAtIndex(serializedNodes.arraySize - 1).managedReferenceValue = n;
            serializedObj.ApplyModifiedProperties();
        }
        public TreeNode MakeTree(Vector2 pos) {

            var tree = new TreeNode() { name = "new Tree", position = pos };
            AddNode(tree);
            OnNodeAdded?.Invoke(tree);
            return tree;
        }
        public Node AddNode(Vector2 pos) {
            var node = new Node() { name = "new Tree", position = pos };
            AddNode(node);
            OnNodeAdded?.Invoke(node);
            return node;
        }

        public void RemoveNode(Node node) {


            var serializedNodes = serializedObj.FindProperty("_nodes");
            var parentList = serializedNodes.GetArrayElementAtIndex(GetNodeSerializedIndex(node.parent)).FindPropertyRelative("nodes");

            //remove from parent
            for (int i = 0; i < parentList.arraySize; i++) {
                var serNode = parentList.GetArrayElementAtIndex(i);
                if (serNode.managedReferenceValue == node) {
                    parentList.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
            this.serializedObj.ApplyModifiedProperties();

            for (int i = 0; i < serializedNodes.arraySize; i++) {
                var transitions = serializedNodes.GetArrayElementAtIndex(i).FindPropertyRelative("_transitions");
                for (int j = transitions.arraySize - 1; j >= 0; j--) {
                    var t = transitions.GetArrayElementAtIndex(j).FindPropertyRelative("target").managedReferenceValue;
                    if (t == node)
                        transitions.DeleteArrayElementAtIndex(j);
                }
            }

            serializedNodes.DeleteArrayElementAtIndex(GetNodeSerializedIndex(node));
            OnNodeRemoved?.Invoke(node);
        }

        public SerializedProperty GetParameter(string name) {
            var p = serializedObj.FindProperty("_parameters");
            for (int i = 0; i < p.arraySize; i++) {
                var z = p.GetArrayElementAtIndex(i);
                if (z.FindPropertyRelative("key").stringValue == name)
                    return z;
            }
            return null;
        }
        public SerializedProperty GetParameter(int index) {
            var p = serializedObj.FindProperty("_parameters");
            return p.GetArrayElementAtIndex(index);
        }
        public void ChangeParmaterIndex(int from, int to) {
            serializedObj.FindProperty("_parameters").MoveArrayElement(from, to);
            this.Apply();
        }

        internal List<SerializedProperty> GetParams() {
            List<SerializedProperty> prop = new();
            var p = serializedObj.FindProperty("_parameters");
            for (int i = 0; i < p.arraySize; i++) {
                prop.Add(p.GetArrayElementAtIndex(i));
            }
            return prop;
        }
        public SerializedProperty GetTransitionAt(int index) {
            return serializedObj.FindProperty("_transitions").GetArrayElementAtIndex(index);
        }
        public void RemoveTransition(Transition t) {
            var index = t.source.transitions.IndexOf(t);
            GetSerializedNode(t.source).FindPropertyRelative("_transitions").DeleteArrayElementAtIndex(index);
        }
        public SerializedProperty GetTransition(Transition t) {
            var index = t.source.transitions.IndexOf(t);
            return GetSerializedNode(t.source).FindPropertyRelative("_transitions").GetArrayElementAtIndex(index);
        }

        public SerializedProperty AddElement(string collection) {
            var c = serializedObj.FindProperty(collection);
            c.arraySize++;
            return c.GetArrayElementAtIndex(c.arraySize - 1);
        }

        public void AddTransition(Transition t) {
            var p = GetSerializedNode(t.source).FindPropertyRelative("_transitions");
            p.arraySize++;
            p.GetArrayElementAtIndex(p.arraySize - 1).managedReferenceValue = t;

        }
        public void Apply() {
            serializedObj.ApplyModifiedProperties();
        }

        public void RemoveParameter(int index) {
            var param = serializedObj.FindProperty("_parameters").GetArrayElementAtIndex(index).managedReferenceValue;

            var nodes = serializedObj.FindProperty("_nodes");
            for (int i = nodes.arraySize - 1; i >= 0; i--) {
                var transitions = nodes.GetArrayElementAtIndex(i).FindPropertyRelative("_transitions");
                for (int j = transitions.arraySize - 1; j >= 0; j--) {
                    var conditions = transitions.GetArrayElementAtIndex(j).FindPropertyRelative("conditions");
                    for (int k = conditions.arraySize - 1; k >= 0; k--) {
                        var p = conditions.GetArrayElementAtIndex(k).FindPropertyRelative("parameter").managedReferenceValue;
                        if (param == p)
                            conditions.DeleteArrayElementAtIndex(k);
                    }
                }

            }

            serializedObj.FindProperty("_parameters").DeleteArrayElementAtIndex(index);
        }

    }

}