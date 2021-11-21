// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// Creates the Custom Map scriptable object and holds the Figma Components to MRTK mapping.
    /// </summary>
    public class FigmaToolkitCustomMap : ScriptableObject
    {
        private const string AssetName = "New Custom Map";
        private const string CustomMapFolder = "Custom Maps";
        private static readonly string FigmaCustomMapPath = Path.Combine(FigmaSettings.FigmaBasePath, CustomMapFolder, $"{AssetName}.asset");


        [MenuItem("Mixed Reality/Toolkit/MRTK Figma Bridge/Create Custom Map")]
        public static FigmaToolkitCustomMap CreateCustomMap()
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder)))
            {
                AssetDatabase.CreateFolder(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder);
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder(Path.Combine(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder, FigmaSettings.GeneratedFolder)))
            {
                AssetDatabase.CreateFolder(Path.Combine(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder), FigmaSettings.GeneratedFolder);
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder(Path.Combine(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder, FigmaSettings.GeneratedFolder, CustomMapFolder)))
            {
                AssetDatabase.CreateFolder(Path.Combine(FigmaSettings.AssetsFolder, FigmaSettings.FigmaFolder, FigmaSettings.GeneratedFolder), CustomMapFolder);
                AssetDatabase.Refresh();
            }

            FigmaToolkitCustomMap newMap = ScriptableObject.CreateInstance<FigmaToolkitCustomMap>();
            newMap.name = AssetName;
            AssetDatabase.CreateAsset(newMap, FigmaCustomMapPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Selection.activeObject = newMap;
            return newMap;
        }

        public TextAsset jsonResponse;
        public TMPro.TMP_FontAsset defaultFont;
        public SerializableDictionary<string, CustomMapItem> componentMap;

        [ContextMenu("Populate Map")]
        private void PopulateComponentsMap()
        {
            GetComponentSets(FigmaFile.FromText(jsonResponse.text));
        }

        private void GetComponentSets(FigmaFile fileResponse)
        {
            foreach (var item in fileResponse.document.children)
            {
                GetComponents(item);
            }
        }
        private void GetComponents(Node node)
        {
            if (node.type == NodeType.ComponentSet || node.type == NodeType.Component)
            {
                componentMap.Add(node.name, new CustomMapItem());
            }

            if (node.children != null && node.type != NodeType.ComponentSet)
            {
                foreach (var child in node.children)
                {
                    GetComponents(child);
                }
            }

        }
    }
}