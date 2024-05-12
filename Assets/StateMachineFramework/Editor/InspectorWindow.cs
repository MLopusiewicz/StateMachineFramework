using StateMachineFramework.View;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class InspectorWindow {
        public VisualElement activeTab;
        VisualElement inspectorTab;
        public InspectorWindow(StateMachineEditor window) {
            window.selection.OnSelectionCleared += Clear;
            inspectorTab = window.rootVisualElement.Q("InspectorTab");
        }

        private void Clear() {
            inspectorTab.SetDisplay(false);
            activeTab?.SetDisplay(false);
            activeTab = null;
        }

        public void SetActive(VisualElement tab) {
            inspectorTab.SetDisplay(true);
            activeTab?.SetDisplay(false);
            activeTab = tab;
            tab.SetDisplay(true);
        }
    }
}