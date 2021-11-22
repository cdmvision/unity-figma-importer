using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class FigmaImporter : EditorWindow
{
    [MenuItem("Window/UI Toolkit/FigmaImporter")]
    public static void ShowExample()
    {
        FigmaImporter wnd = GetWindow<FigmaImporter>();
        wnd.titleContent = new GUIContent("FigmaImporter");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);
    }
}