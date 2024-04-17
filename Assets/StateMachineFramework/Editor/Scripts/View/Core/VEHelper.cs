using System;
using UnityEngine.UIElements;

public static class VEHelper {
    public static void SetDisplay(this VisualElement ve, bool state) {
        if (state)
            ve.style.display = DisplayStyle.Flex;
        else
            ve.style.display = DisplayStyle.None;

    }
    public static T Make<T>(VisualElement parent, string name, params string[] styles) where T : VisualElement, new() {
        var ve = new T();
        foreach (var style in styles)
            ve.AddToClassList(style);
        ve.name = name;
        parent.Add(ve);
        return ve;
    }
}
