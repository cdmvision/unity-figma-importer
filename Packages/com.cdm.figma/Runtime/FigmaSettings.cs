using System.IO;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// Scriptable Object holding Figma settings, auto created when needed.
    /// </summary>
    public class FigmaSettings : ScriptableObject
    {
        public const string FigmaFolder = "Figma";
        public const string GeneratedFolder = "Resources";
        public const string AssetsFolder = "Assets";
        public static readonly string FigmaBasePath = Path.Combine(AssetsFolder, FigmaFolder, GeneratedFolder);
        public const string FigmaStylesheetPath = "Packages/com.cdm.figma/Editor/FigmaSettingsInspector.uss";

        private const string AssetName = nameof(FigmaSettings);
        private static readonly string FigmaSettingsPath = Path.Combine(FigmaBasePath, $"{AssetName}.asset");

        public const float PositionScale = 0.00033333f;



        public string FigmaToken;


        public static bool TryLoadProjectSettings(out FigmaSettings settings)
        {
            settings = Resources.Load<FigmaSettings>(AssetName);
            return settings != null;
        }

        public static FigmaSettings GetOrCreateSettings()
        {
#if UNITY_EDITOR
            return GetOrCreateSettingsEditor();
#endif
        }

#if UNITY_EDITOR
        private static FigmaSettings GetOrCreateSettingsEditor()
        {
            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<FigmaSettings>(FigmaSettingsPath);
            if (settings == null)
            {
                if (!UnityEditor.AssetDatabase.IsValidFolder(Path.Combine(AssetsFolder, FigmaFolder)))
                {
                    UnityEditor.AssetDatabase.CreateFolder(AssetsFolder, FigmaFolder);
                    UnityEditor.AssetDatabase.Refresh();
                }

                if (!UnityEditor.AssetDatabase.IsValidFolder(Path.Combine(AssetsFolder, FigmaFolder, GeneratedFolder)))
                {
                    UnityEditor.AssetDatabase.CreateFolder(Path.Combine(AssetsFolder, FigmaFolder), GeneratedFolder);
                    UnityEditor.AssetDatabase.Refresh();
                }

                var newSettings = CreateInstance<FigmaSettings>();
                newSettings.name = AssetName;
                UnityEditor.AssetDatabase.CreateAsset(newSettings, FigmaSettingsPath);
                UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
                settings = UnityEditor.AssetDatabase.LoadAssetAtPath<FigmaSettings>(FigmaSettingsPath);
            }

            return settings;
        }
#endif
    }
}