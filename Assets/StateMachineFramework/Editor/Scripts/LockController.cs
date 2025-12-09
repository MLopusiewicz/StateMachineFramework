using UnityEngine;
using UnityEngine.UIElements;

public class LockController : MonoBehaviour {

    public bool IsLocked { get; private set; }
    Toggle lockToggle;
    public LockController(StateMachineEditor w) {
        lockToggle = w.rootVisualElement.Q<Toggle>("LockButton");
        lockToggle.RegisterValueChangedCallback<bool>(OnLockChanged);
    }

    private void OnLockChanged(ChangeEvent<bool> evt) {
        IsLocked = evt.newValue;
    }
}
