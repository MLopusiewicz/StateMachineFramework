using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class TransitionInspector {

        public ConditionInspector conditions;

        StateMachineEditor editor;
        VisualElement container;

        ListView transitionsList;
        List<Transition> displayedTransitions;

        public TransitionInspector(StateMachineEditor editor) {
            this.editor = editor;
            container = editor.rootVisualElement.Q(name: "TransitionInspector");
            conditions = new ConditionInspector(container, editor);
            container.SetDisplay(false);
            SetupTransitionsList();
        }

        public void Display(List<Transition> transitions) {

            displayedTransitions = transitions;
            if (transitions == null) {
                this.Clear();
                return;
            }
            if (transitions.Count > 0) {
                editor.inspector.SetActive(container);
                conditions.ShowConditions(transitions[0]);
                transitionsList.itemsSource = transitions;
                transitionsList.Rebuild();
                transitionsList.SetSelection(0);
            }
        }

        public void Clear() {
            transitionsList.itemsSource = null;
            transitionsList.Clear();
            transitionsList.Rebuild();
            conditions.Clear();
        }

        void SetupTransitionsList() {
            transitionsList = container.Q<ListView>(name: "Transitions");
            transitionsList.makeItem = () => {
                return new Label();
            };
            transitionsList.bindItem = (ve, i) => {
                var c = ve.Q<Label>();
                var t = (transitionsList.itemsSource[i] as Transition);
                string sourceText;
                sourceText = t.source.name;
                c.text = $"{sourceText} -> {t.target.name}";

                if (t.source == editor.stateMachine.AnyState && t.conditions.Count == 0)
                    c.AddToClassList("error-label");
            };
            transitionsList.itemsRemoved += TransitionRemoved;
            transitionsList.selectionChanged += TransitionSelected;
            transitionsList.Q<Button>(name: "unity-list-view__add-button").SetDisplay(false);

        }

        private void TransitionRemoved(IEnumerable<int> enumerable) {
            List<int> removal = new List<int>(enumerable);
            removal.Reverse();
            foreach (var a in removal) {
                editor.serialization.RemoveTransition(displayedTransitions[a]);
                editor.serialization.Apply();
            }


            editor.transitions.Redraw();
            //Redraw();
        }

        private void TransitionSelected(IEnumerable<object> enumerable) {
            foreach (var a in enumerable) {
                conditions.ShowConditions(a as Transition);
            }
        }


        public void Redraw() {

            Display(editor.selection.transitions.GetItems());

        }
    }
}