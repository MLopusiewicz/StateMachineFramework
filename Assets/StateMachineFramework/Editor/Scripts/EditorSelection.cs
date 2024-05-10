using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class EditorSelection {
        ViewPortVE viewPort;
        public event Action OnSelectionCleared;
        public event Action<Node> OnNodeSelected;

        SMWindow w;

        public SerializedSelection<Transition> transitions;


        public EditorSelection(SMWindow w) {
            viewPort = w.rootVisualElement.Q<ViewPortVE>();
            viewPort.RegisterCallback<MouseDownEvent>(OnClicked);
            this.w = w;
            transitions = new SerializedSelection<Transition>(w.serialization.GetTransition);
        }

        void OnClicked(MouseDownEvent e) {
            if (e.target == viewPort) {
                if (e.button == 0)
                    ClearSelection();
            }
        }

        private void ClearSelection() {
            OnSelectionCleared?.Invoke();
        }

    }

    public class SerializedSelection<T> {

        List<SerializedProperty> _data = new();
        Func<T, SerializedProperty> parseFunc;
        public SerializedSelection(Func<T, SerializedProperty> parseFunc) {
            this.parseFunc = parseFunc;
        }

        public List<T> GetItems() {
            for (int i = _data.Count - 1; i >= 0; i--) {
                try {
                    if (_data[i] == null || _data[i].managedReferenceValue == null)
                        _data.RemoveAt(i);
                } catch {
                    _data.RemoveAt(i);
                }
            }

            List<T> list = new List<T>();
            foreach (var a in _data) {
                list.Add((T)a.managedReferenceValue);
            }
            return list;
        }
        public void Add(T value) {
            _data.Add(parseFunc(value));
        }
        public void Remove(T value) {
            _data.Remove(parseFunc(value));
        }

        public void Clear() {
            _data.Clear();
        }

        public bool Contains(Transition a) {
            foreach (var item in _data) {
                if (item.managedReferenceValue == a)
                    return true;
            }
            return false;
        }
    }
}