using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class ConditionInspector {
        ListView conditionsList;
        VisualElement container;
        SearchPopupVE searchPopup;
        int searchedIndex;

        VisualElement emptyTransitionError;

        Transition displayedTransition;
        StateMachineEditor editor;
        public ConditionInspector(VisualElement root, StateMachineEditor editor) {
            container = root;
            searchPopup = container.Q<SearchPopupVE>();
            emptyTransitionError = container.Q(name: "AnyStateError");
            emptyTransitionError.SetDisplay(false);

            searchPopup.Hide();
            SetupConditionsList();

            this.editor = editor;
        }

        public void ShowConditions(Transition transition) {
            displayedTransition = transition;

            conditionsList.itemsSource = editor.serialization.TranstionConditions(displayedTransition);
            conditionsList.RefreshItems();
            searchPopup.Init(editor.stateMachine.GetAllParameters.Select(x => x.Key).ToList());


            if (transition.source == editor.stateMachine.anyState && transition.conditions.Count == 0)
                emptyTransitionError.SetDisplay(true);
            else
                emptyTransitionError.SetDisplay(false);


        }

        void SetupConditionsList() {
            conditionsList = container.Q<ListView>(name: "Conditions");
            conditionsList.makeItem = () => {
                return new ConditionVE();
            };
            conditionsList.bindItem = (ve, i) => {
                var c = ve.Q<ConditionVE>();
                var p = editor.serialization.ConditionsList(displayedTransition).GetArrayElementAtIndex(i);
                c.Init(p, () => ChangeRequested(ve, i));
            };

            conditionsList.itemsAdded += Add;
            conditionsList.itemsRemoved += OnItemRemoved;
            conditionsList.itemIndexChanged += OnReordered;
        }


        void Add(IEnumerable<int> a) {

            var g = editor.serialization.ConditionsList(displayedTransition);
            g.arraySize++;
            UpdateParameter(editor.stateMachine.GetAllParameters[0].Key, g.arraySize - 1);
            editor.serialization.Apply();
        }

        void ChangeRequested(VisualElement ve, int index) {
            searchedIndex = index;
            searchPopup.Show();
            var allignTarget = ve.Q<Button>();

            searchPopup.transform.position = ve.parent.parent.layout.position;
            searchPopup.transform.position += Vector3.up * searchPopup.layout.height;
            searchPopup.transform.position += Vector3.right * 33;

            searchPopup.style.width = ve.layout.width;

            searchPopup.OnEntrySelected += ApplyChange;
        }

        void ApplyChange(string obj) {
            UpdateParameter(obj, searchedIndex);
        }

        void OnReordered(int arg1, int arg2) {
            editor.serialization.ConditionsList(displayedTransition).MoveArrayElement(arg1, arg2);
            editor.serialization.Apply();
        }

        void OnItemRemoved(IEnumerable<int> enumerable) {

            List<int> removalList = new List<int>(enumerable);
            removalList.Reverse();
            foreach (int i in removalList) {
                editor.serialization.ConditionsList(displayedTransition).DeleteArrayElementAtIndex(i);
            }
            editor.serialization.Apply();
        }


        void UpdateParameter(string parmater, int conditionIndex) {
            var param = editor.serialization.GetParameter(parmater);

            var ss = editor.serialization.GetTransition(displayedTransition).FindPropertyRelative("conditions");
            var transitionCondition = ss.GetArrayElementAtIndex(conditionIndex);

            Equation g;
            var paramVal = param.managedReferenceValue;
            if (paramVal is TriggerParameter)
                g = new TriggerEquation();
            else if (paramVal is BoolParameter)
                g = new BoolEquation();
            else if (paramVal is FloatParameter)
                g = new FloatEquation();
            else
                g = new IntEquation();

            transitionCondition.FindPropertyRelative("parameter").managedReferenceId = param.managedReferenceId;
            transitionCondition.FindPropertyRelative("equation").managedReferenceValue = g;
            transitionCondition.serializedObject.ApplyModifiedProperties();
            conditionsList.RefreshItems();
            Redraw();
        }
        public void Redraw() {
            if (displayedTransition != null)
                ShowConditions(displayedTransition);
        }

        public void Clear() {

            conditionsList.itemsSource = null;
            conditionsList.RefreshItems();
        }
    }
}