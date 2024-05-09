using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class TransitionInspector {
        SMWindow w;
        VisualElement container;

        ListView transitionsList;
        Transition activeTransition;
        ConditionInspector conditions;
        List<Transition> displayedTransitions;
        public TransitionInspector(SMWindow w) {
            this.w = w;
            container = w.rootVisualElement.Q(name: "TransitionInspector");
            conditions = new ConditionInspector(container, w);
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
                w.inspector.SetActive(container);
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

                if (t.source == w.stateMachine.anyState && t.conditions.Count == 0)
                    c.AddToClassList("error-label");
            };
            transitionsList.itemsRemoved += TransitionRemoved;
            transitionsList.selectionChanged += TransitionSelected;
            transitionsList.Q<Button>(name: "unity-list-view__add-button").SetDisplay(false);

        }

        private void TransitionRemoved(IEnumerable<int> enumerable) {
            List<int> removal = new List<int>(enumerable);
            removal.Reverse();
            foreach (var a in removal)
                w.serialization.RemoveTransition(displayedTransitions[a]);

            w.serialization.Apply();
            Redraw();
            w.transitions.Redraw();
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