using UnityEditor;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CustomEditor(typeof(FigmaFile))]
    public class FigmaFileEditor : Cdm.Figma.FigmaFileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
           
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{PackageUtils.VisualTreeFolder}/UIToolkit/FigmaFile.uxml");

            visualTree.CloneTree(root);
            return root;
        }
    }
}