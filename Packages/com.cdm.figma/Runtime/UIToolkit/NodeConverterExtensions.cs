using System.IO;
using Cdm.Figma.UIToolkit;
using UnityEngine;

namespace Cdm.Figma
{
    public static class NodeConverterExtensions
    {
        public static bool TryGetGraphicStyleUrl(this NodeConvertArgs args, string id, out string url)
        {
#if UNITY_EDITOR
            if (args.graphics.TryGetValue(id, out var graphic))
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                url = $"url(\"project:///{assetPath}\")";
                return true;
            }
#else
            Debug.LogError("Cannot determine asset path at runtime.");
#endif

            url = null;
            return false;
        }

        public static bool TryGetGraphicPath(this NodeConvertArgs args, string id, out string url)
        {
#if UNITY_EDITOR
            if (args.graphics.TryGetValue(id, out var graphic))
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(graphic);
                url = Path.Combine(Application.dataPath, assetPath);
                return true;
            }
#else
            Debug.LogError("Cannot determine asset path at runtime.");
#endif

            url = null;
            return false;
        }
        
        public static bool TryGetFontStyleUrl(this NodeConvertArgs args, FontDescriptor fontDescriptor, out string url)
        {
#if UNITY_EDITOR
            if (args.fonts.TryGetValue(fontDescriptor, out var font))
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(font);
                url = $"url(\"project:///{assetPath}\")";
                return true;
            }
#else
            Debug.LogError("Cannot determine font path at runtime.");
#endif

            url = null;
            return false;
        }
        
        public static bool TryGetFontPath(this NodeConvertArgs args, FontDescriptor fontDescriptor, out string url)
        {
#if UNITY_EDITOR
            if (args.fonts.TryGetValue(fontDescriptor, out var font))
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(font);
                url = Path.Combine(Application.dataPath, assetPath);
                return true;
            }
#else
            Debug.LogError("Cannot determine font path at runtime.");
#endif

            url = null;
            return false;
        }
    }
}