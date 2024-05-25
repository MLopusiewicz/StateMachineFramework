using StateMachineFramework.Editor;
using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TargetSMWindow : SMWindow {

    public static List<TargetSMWindow> windowsList = new();

    public StateMachineFramework.Runtime.StateMachine target;
    public override void CreateGUI() {
        base.CreateGUI();
        windowsList.Add(this);
        editor.SetDisplay(target);
    }


    public void Init(StateMachineFramework.Runtime.StateMachine sm) {
        this.titleContent = new GUIContent(sm.gameObject.name, this.icon);
        this.target = sm;
    }
    void OnDestroy() {
        windowsList.Remove(this);
    }
}