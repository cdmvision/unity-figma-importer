// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// Scriptable object that holds a list of file downloaded from the Figma API
    /// </summary>
    public class FigmaToolkitData : ScriptableObject
    {
        private const string AssetName = "FigmaData";
        private static readonly string FigmaDataPath = Path.Combine(FigmaSettings.FigmaBasePath, $"{AssetName}.asset");

        public List<FigmaFileAsset> files;
        public string lastRequestedFile;
        public string lastDocumentOpened;

        private static void ShowData()
        {
            Selection.activeObject = EditorGetOrCreateData();
        }
        public static bool TryLoadFigmaData(out FigmaToolkitData data)
        {
            data = Resources.Load<FigmaToolkitData>(AssetName);
            return data != null;
        }
        public static FigmaToolkitData EditorGetOrCreateData()
        {
            var data = AssetDatabase.LoadAssetAtPath<FigmaToolkitData>(FigmaDataPath);
            if (data == null)
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

                FigmaToolkitData newData = ScriptableObject.CreateInstance<FigmaToolkitData>();
                newData.name = AssetName;
                AssetDatabase.CreateAsset(newData, FigmaDataPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                data = AssetDatabase.LoadAssetAtPath<FigmaToolkitData>(FigmaDataPath);
            }

            return data;
        }

    }
}