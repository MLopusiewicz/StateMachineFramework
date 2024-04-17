﻿using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class ConditionInspector {
        ListView conditionsList;
        VisualElement container;
        SearchPopupVE searchPopup;
        int searchedIndex;

        VisualElement emptyTransitionError;

        Transition displayedTransition;
        Window w;
        public ConditionInspector(VisualElement root, Window w) {
            container = root;
            searchPopup = container.Q<SearchPopupVE>();
            emptyTransitionError = container.Q(name: "AnyStateError");
            emptyTransitionError.SetDisplay(false);

            searchPopup.Hide();
            SetupConditionsList();

            this.w = w;
        }
        public void ShowConditions(Transition transition) {
            displayedTransition = transition;

            conditionsList.itemsSource = w.serialization.TranstionConditions(displayedTransition);
            conditionsList.RefreshItems();
            searchPopup.Init(w.stateMachine.Parameters.Select(x => x.Key).ToList());


            if (transition.source == w.stateMachine.anyState && transition.conditions.Count == 0)
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
                var p = conditionsList.itemsSource[i] as SerializedProperty;
                c.Init(p, () => ChangeRequested(ve, i));
            };


            conditionsList.itemsAdded += OnItemAdded;
            conditionsList.itemsRemoved += OnItemRemoved;
            conditionsList.itemIndexChanged += OnReordered;
        }
        private void Add() {
            var g = w.serialization.ConditionsList(displayedTransition);
            g.arraySize++;
            UpdateParameter(w.stateMachine.Parameters[0].Key, g.arraySize - 1);
            w.serialization.Apply();
        }
        void ChangeRequested(VisualElement ve, int index) {
            searchedIndex = index;
            searchPopup.Show();
            var allignTarget = ve.Q<Button>();
            searchPopup.transform.position = ve.localBound.position;// + Vector2.up * ve.localBound.height;
            searchPopup.OnEntrySelected += ApplyChange;
        }

        void ApplyChange(string obj) {
            UpdateParameter(obj, searchedIndex);
        }

        void OnReordered(int arg1, int arg2) {
            w.serialization.ConditionsList(displayedTransition).MoveArrayElement(arg1, arg2);
            w.serialization.Apply();

            conditionsList.itemsSource = w.serialization.TranstionConditions(displayedTransition);
            conditionsList.RefreshItems();
        }

        void OnItemRemoved(IEnumerable<int> enumerable) {

            foreach (int i in enumerable) {
                w.serialization.ConditionsList(displayedTransition).DeleteArrayElementAtIndex(i);
            }

            w.serialization.Apply();
            conditionsList.itemsSource = w.serialization.TranstionConditions(displayedTransition);
            conditionsList.RefreshItems();
        }

        void OnItemAdded(IEnumerable<int> enumerable) {
            Add();
            conditionsList.itemsSource = w.serialization.TranstionConditions(displayedTransition);
            conditionsList.RefreshItems();
        }

        void UpdateParameter(string parmater, int conditionIndex) {
            var param = w.serialization.GetParameter(parmater);

            var ss = w.serialization.GetTransition(displayedTransition).FindPropertyRelative("conditions");
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
            ShowConditions(displayedTransition);
        }
        public void Clear() {

            conditionsList.itemsSource = null;
            conditionsList.RefreshItems();

        }
    }
}