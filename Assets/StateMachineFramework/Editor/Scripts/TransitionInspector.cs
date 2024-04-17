using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class TransitionInspector {
        Window w;
        VisualElement container;

        ListView transitionsList;
        Transition activeTransition;
        ConditionInspector conditions;

        public TransitionInspector(Window w) {
            this.w = w;
            container = w.rootVisualElement.Q(name: "TransitionInspector");
            conditions = new ConditionInspector(container, w);
            SetupTransitionsList();
        }

        public void Display(List<Transition> transitions) {
            if (transitions == null) {
                this.Clear();
                return;
            }
            if (transitions.Count > 0) {
                conditions.ShowConditions(transitions[0]);

                transitionsList.itemsSource = transitions;
                transitionsList.Rebuild();
                transitionsList.SetSelection(0);
            } else {
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

                if (t.source == w.stateMachine.anyState && t.conditions.Count == 0)
                    c.AddToClassList("error-label");
            };
            transitionsList.itemsRemoved += TransitionRemoved;
            transitionsList.Q<Button>(name: "unity-list-view__add-button").SetDisplay(false);
            transitionsList.selectionChanged += TransitionSelected;

        }

        private void TransitionRemoved(IEnumerable<int> enumerable) {
            foreach (var a in enumerable)
                w.serialization.RemoveTransition(transitionsList.itemsSource[a] as Transition);

            w.serialization.Apply();
            w.transitions.Redraw();
            Redraw();
        }

        private void TransitionSelected(IEnumerable<object> enumerable) {
            foreach (var a in enumerable) {
                conditions.ShowConditions(a as Transition);
            }
        }


        public void Redraw() {

            List<Transition> transitions = new List<Transition>();
            if (activeTransition != null)
                foreach (var a in activeTransition.source.transitions) {
                    if (a.target == activeTransition.target)
                        transitions.Add(a);
                }
            Display(transitions);

        }
    }
}