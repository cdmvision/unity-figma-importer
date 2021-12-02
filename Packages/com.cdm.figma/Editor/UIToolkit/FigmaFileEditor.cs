using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
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

            root.Q<Button>("addFontsButton").clicked += () =>
            {
                var path = EditorUtility.OpenFolderPanel("Select folder to be searched", "", "Assets");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = path.Replace("\\", "/").Replace(Application.dataPath.Replace("\\", "/"), "");
                    if (path.Length > 0 && path[0] == '/')
                    {
                        path = path.Substring(1, path.Length - 1);
                    }
                    path = Path.Combine("Assets", path);
                    
                    var guids = AssetDatabase.FindAssets($"t:{typeof(FontAsset)}", new []{path});
                    var assets = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
                    
                    var figmaFile = (FigmaFile) target;

                    foreach (var fontSource in figmaFile.fonts)
                    {
                        if (fontSource.font == null)
                        {
                            if (TryGetFontAsset(assets, fontSource, out var fontAsset))
                            {
                                fontSource.font = fontAsset;
                            
                                Debug.Log($"{fontSource} was mapped to {AssetDatabase.GetAssetPath(fontSource.font)}");
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                    
                    AssetDatabase.SaveAssetIfDirty(target);
                }
            };
            
            return root;
        }

        private static bool TryGetFontAsset(string[] assets, FontDescriptor fontDescriptor, out FontAsset fontAsset)
        {
            foreach (var assetPath in assets)
            {
                var assetName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                if (assetName.Contains(fontDescriptor.family.ToLower()) && 
                    assetName.Contains(fontDescriptor.weight.ToString().ToLower()) &&
                    (!fontDescriptor.italic || assetName.Contains("italic")))
                {
                    fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(assetPath);
                    return fontAsset != null;
                }
            }

            fontAsset = null;
            return false;
        }
    }
}