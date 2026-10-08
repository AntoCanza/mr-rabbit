using System;
using System.Reflection;
using UnityEditor;

// Used by the Unity CLI for repeatable screen-size checks; not included in builds.
public static class PreviewResolution
{
    public static string Run(int width, int height)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var sizesType = Type.GetType("UnityEditor.GameViewSizes,UnityEditor");
        var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var sizes = singletonType.GetProperty("instance", flags | BindingFlags.FlattenHierarchy).GetValue(null);
        var group = sizesType.GetProperty("currentGroup", flags).GetValue(sizes);
        var groupType = group.GetType();
        int count = (int)groupType.GetMethod("GetTotalCount", flags).Invoke(group, null);
        string label = "Mr Rabbit " + width + " x " + height;
        int index = -1;
        var sizeType = Type.GetType("UnityEditor.GameViewSize,UnityEditor");
        for (int i = 0; i < count; i++)
        {
            var size = groupType.GetMethod("GetGameViewSize", flags).Invoke(group, new object[] { i });
            if ((string)sizeType.GetProperty("baseText", flags).GetValue(size) == label) { index = i; break; }
        }
        if (index < 0)
        {
            var kind = Enum.Parse(Type.GetType("UnityEditor.GameViewSizeType,UnityEditor"), "FixedResolution");
            var size = Activator.CreateInstance(sizeType, flags, null, new object[] { kind, width, height, label }, null);
            groupType.GetMethod("AddCustomSize", flags).Invoke(group, new[] { size });
            index = count;
        }
        var viewType = Type.GetType("UnityEditor.GameView,UnityEditor");
        var view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", flags).SetValue(view, index);
        view.Focus();
        view.Repaint();
        EditorApplication.delayCall += () =>
        {
            float scale = (float)viewType.GetProperty("minScale", flags).GetValue(view);
            viewType.GetMethod("SnapZoom", flags).Invoke(view, new object[] { scale });
            view.Repaint();
        };
        return label;
    }
}
