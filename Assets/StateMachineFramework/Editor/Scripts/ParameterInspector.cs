using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static StateMachineFramework.Runtime.ParameterController;

namespace StateMachineFramework.Editor {
    public class ParameterInspector {


        VisualElement tab;
        bool tabVisible = false;
        private SMWindow w;
        ToolbarPopupSearchField searchBar;
        Dictionary<IParameter, VisualElement> paramVE = new();
        public Action<IParameter> ValueChanged;
        DropdownField addDropdown;
        ListView paramList;
        Button tabButton;

        ParameterType visibleParams = (ParameterType)int.MaxValue;

        Dictionary<Type, ParameterType> paramTypeLUT = new() {
        {typeof(TriggerParameter), ParameterType.Trigger},
        {typeof(FloatParameter), ParameterType.Float},
        {typeof(IntParameter), ParameterType.Int},
        {typeof(BoolParameter), ParameterType.Bool},
    };

        public ParameterInspector(SMWindow window) {
            tabButton = window.rootVisualElement.Q<Button>(name: "ParameterTabButton");
            tabButton.clicked += ToggleTab;
            tab = window.rootVisualElement.Q(name: "ParameterTab");
            searchBar = tab.Q<ToolbarPopupSearchField>();

            this.w = window;
            searchBar.RegisterCallback<ChangeEvent<string>>(SearchBarChanged);

            SubstituteFooterAddButton();
            ToggleTab();
            SetupMenu();
            ListSetup();

        }
        void SubstituteFooterAddButton() {
            addDropdown = new DropdownField();
            addDropdown.AddToClassList("parameter-add-dropdown");
            tab.Q(name: "unity-list-view__footer").Add(addDropdown);
            addDropdown.choices = new List<string> { "Trigger", "Int", "Float", "Bool" };
            addDropdown.SetValueWithoutNotify("+");

            addDropdown.RegisterCallback<ChangeEvent<string>>(DropdownSelected);
            tab.Q<Button>(name: "unity-list-view__add-button").SetDisplay(false);
        }

        void ListSetup() {
            paramList = tab.Q<ListView>();

            paramList.makeItem = () => {
                return new ParameterVE();
            };
            paramList.bindItem = (ve, index) => {
                var v = ve.Q<ParameterVE>();
                if (!w.isRuntime)
                    v.Init(w.serialization.GetParameter(index));
                else
                    v.InitRuntime(paramList.itemsSource[index] as IParameter);
            };

            paramList.itemIndexChanged += OnReordered;
            paramList.itemsRemoved += Removed;
        }

        private void Removed(IEnumerable<int> enumerable) {

            List<int> indexes = new(enumerable);
            indexes.Reverse();
            foreach (var i in indexes) {
                w.serialization.RemoveParameter(i);
            }
            w.serialization.Apply();
        }

        void OnReordered(int arg1, int arg2) {
            w.serialization.ChangeParmaterIndex(arg1, arg2);
            RefreshList();
        }

        private void DropdownSelected(ChangeEvent<string> evt) {
            addDropdown.SetValueWithoutNotify("+");
            IParameter parameter = new TriggerParameter("New trigger", false);
            switch (evt.newValue) {
                case "Bool":
                    parameter = new BoolParameter("New boolean", false);
                    break;
                case "Float":
                    parameter = new FloatParameter("New float", 0);
                    break;
                case "Int":
                    parameter = new IntParameter("New int", 0);
                    break;
            }

            var serializedField = w.serialization.AddElement("_parameters");
            serializedField.managedReferenceValue = parameter;
            w.serialization.Apply();
            RefreshList();
        }

        void SetupMenu() {
            searchBar.menu.ClearItems();
            searchBar.menu.AppendAction("All", (x) => { visibleParams = (ParameterType)int.MaxValue; SetupMenu(); Redraw(); }, DropdownMenuAction.Status.Normal);
            searchBar.menu.AppendAction("None", (x) => { visibleParams = 0; SetupMenu(); Redraw(); }, DropdownMenuAction.Status.Normal);
            searchBar.menu.AppendSeparator("");
            foreach (var a in (ParameterType[])Enum.GetValues(typeof(ParameterType))) {
                searchBar.menu.AppendAction(a.ToString(), (x) => ToggleParameter(x, a), visibleParams.HasFlag(a) ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }
        }

        private void ToggleParameter(DropdownMenuAction x, ParameterType p) {
            if (visibleParams.HasFlag(p))
                visibleParams &= ~p;
            else {
                visibleParams |= p;
            }
            SetupMenu();
            Redraw();
        }

        private void ToggleTab() {
            tab.EnableInClassList("hidden", tabVisible ^= true);
        }

        public void Init() {
            paramVE.Clear();
            RefreshList();
        }

        private void SearchBarChanged(ChangeEvent<string> evt) {
            Redraw();
        }

        void Redraw() {
            ParameterType search = visibleParams;
            string searchString = searchBar.value;

            if (searchBar.value.StartsWith("t:")) {
                search = ParameterType.Trigger;
                searchString = searchString.Remove(0, 2);
            }
            if (searchBar.value.StartsWith("f:")) {
                search = ParameterType.Float;
                searchString = searchString.Remove(0, 2);
            }
            if (searchBar.value.StartsWith("i:")) {
                search = ParameterType.Int;
                searchString = searchString.Remove(0, 2);
            }
            if (searchBar.value.StartsWith("b:")) {
                search = ParameterType.Bool;
                searchString = searchString.Remove(0, 2);
            }


            for (int i = 0; i < w.stateMachine.Parameters.Count; i++) {
                var param = w.stateMachine.Parameters[i];
                var t = paramTypeLUT[param.GetType()];
                if (param.Key.Contains(searchString) && search.HasFlag(t)) {
                    paramList.GetRootElementForIndex(i).SetDisplay(true);
                } else
                    paramList.GetRootElementForIndex(i).SetDisplay(false);
            }

        }

        void RefreshList() {
            paramList.itemsSource = new List<IParameter>(w.stateMachine.Parameters);
            paramList.Rebuild();
        }

        internal void Clear() {
        }
    }
}