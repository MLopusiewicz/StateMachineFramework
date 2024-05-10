using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class SearchPopupVE : VisualElement {

        public const string SEARCH_POPUP = "search-popup";
        public const string SEARCH_POPUP_BAR = "search-popup-bar";
        public const string SEARCH_POPUP_SEPARATOR = "search-popup-separator";
        public const string SEARCH_POPUP_LIST = "search-popup-list";

        public class SearchPopupFactory : UxmlFactory<SearchPopupVE, SearchPopupTraits> { }
        public class SearchPopupTraits : UxmlTraits {
            public UxmlStringAttributeDescription entries = new UxmlStringAttributeDescription() { name = "Entries", defaultValue = "a,b,c,d" };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<SearchPopupVE>();
                target.Init(entries.GetValueFromBag(bag, cc).Split(",").ToList());
            }
        }

        ToolbarSearchField searchBar;
        ListView behaviourList;

        public Action<string> OnEntrySelected;
        List<string> allItems;
        string searchKey = "";

        public SearchPopupVE() {
            SetupContainters();

            searchBar.RegisterCallback<ChangeEvent<string>>(SearchChanged);

            behaviourList.makeItem = () => {
                return new Label();
            };
            behaviourList.bindItem = (ve, index) => {
                ve.Q<Label>().text = behaviourList.itemsSource[index] as string;
            };
            behaviourList.selectionChanged += OnSelected;
            this.RegisterCallback<FocusOutEvent>(FocusLost);
        }


        public void SetupContainters() {
            this.AddToClassList(SEARCH_POPUP);
            searchBar = VEHelper.Make<ToolbarSearchField>(this, "SearchBar", SEARCH_POPUP_BAR);
            VEHelper.Make<VisualElement>(this, "Separator", SEARCH_POPUP_SEPARATOR);
            behaviourList = VEHelper.Make<ListView>(this, "EntryList", SEARCH_POPUP_LIST);
        }


        public void Show() {
            this.visible = true;
            searchBar.Focus();
        }
        public void Hide() {
            this.visible = false;
        }

        void Redraw() {
            behaviourList.itemsSource = allItems.Where(x => x.Contains(searchKey)).ToList();
            behaviourList.Rebuild();
        }

        internal void Init(List<string> list) {
            allItems = list;
            Redraw();
        }


        private void SearchChanged(ChangeEvent<string> evt) {
            searchKey = evt.newValue;
            Redraw();
        }

        private void FocusLost(FocusOutEvent evt) {
            Hide();
        }

        private void OnSelected(IEnumerable<object> enumerable) {
            foreach (var a in enumerable) {
                OnEntrySelected?.Invoke(a.ToString());
                Hide();
                behaviourList.ClearSelection();
                return;
            }

        }
    }


}