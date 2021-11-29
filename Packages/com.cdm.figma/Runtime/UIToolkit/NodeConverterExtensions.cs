using System.IO;
using Cdm.Figma.UIToolkit;
using UnityEngine;

namespace Cdm.Figma
{
    public static class NodeConverterExtensions
    {
        public static bool TryGetStyleUrl(this NodeConvertArgs args, string id, out string url)
        {
            if (args.assets.TryGetValue(id, out var assetPath))
            {
                url = $"url(project:///Assets/{assetPath})";
                return true;
            }

            url = null;
            return false;
        }
        
        public static bool TryGetAssetPath(this NodeConvertArgs args, string id, out string url)
        {
            if (args.assets.TryGetValue(id, out var assetPath))
            {
                url = Path.Combine(Application.dataPath, assetPath);
                return true;
            }

            url = null;
            return false;
        }
    }
}