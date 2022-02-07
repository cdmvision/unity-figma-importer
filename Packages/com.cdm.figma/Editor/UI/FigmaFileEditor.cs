using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UI
{
    [CustomEditor(typeof(FigmaFile))]
    public class FigmaFileEditor : Cdm.Figma.FigmaFileEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
           
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{PackageUtils.VisualTreeFolderPath}/UIToolkit/FigmaFile.uxml");
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
                    
                    var guids = AssetDatabase.FindAssets($"t:{typeof(TMP_FontAsset)}", new []{path});
                    var assets = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
                    
                    var figmaFile = (FigmaFile) target;

                    foreach (var fontSource in figmaFile.fonts)
                    {
                        if (fontSource.font == null)
                        {
                            if (TryGetFontAsset(assets, fontSource.fontName, out var fontAsset))
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

        private static bool TryGetFontAsset(string[] assets, string fontName, out TMP_FontAsset fontAsset)
        {
            foreach (var assetPath in assets)
            {
                var assetName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                var tokens = fontName.Split("-");

                if (tokens.All(t => assetName.Contains(t.ToLower())))
                {
                    fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
                    return fontAsset != null;
                }
            }

            fontAsset = null;
            return false;
        }
    }
}