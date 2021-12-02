using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class FigmaFile : Figma.FigmaFile
    {
        [SerializeField]
        private List<GraphicSource> _graphics = new List<GraphicSource>();
        
        /// <summary>
        /// Gets the graphic assets. 
        /// </summary>
        public IList<GraphicSource> graphics => _graphics;
        
        [SerializeField]
        private List<FontSource> _fonts = new List<FontSource>();
        
        /// <summary>
        /// Gets the font assets.
        /// </summary>
        public IList<FontSource> fonts => _fonts;

        public bool TryGetGraphic(string graphicId, out VectorImage graphic)
        {
            var graphicSource = _graphics.FirstOrDefault(x => x.id == graphicId);
            if (graphicSource != null)
            {
                graphic = graphicSource.graphic;
                return true;
            }

            graphic = null;
            return false;
        }

        public bool TryGetGraphicUrl(string graphicId, out string url)
        {
#if UNITY_EDITOR
            if (TryGetGraphic(graphicId, out var graphic))
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
        
        public bool TryGetFont(FontDescriptor fontDescriptor, out FontAsset font)
        {
            
            var fontSource = _fonts.FirstOrDefault(x => x.Equals(fontDescriptor));
            if (fontSource != null)
            {
                font = fontSource.font;
                return font != null;
            }

            font = null;
            return false;
        }
        
        public bool TryGetFontUrl(FontDescriptor fontDescriptor, out string url)
        {
#if UNITY_EDITOR
            if (TryGetFont(fontDescriptor, out var font))
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(font);
                url = $"url(\"project:///{assetPath}\")";
                return true;
            }
#else
            Debug.LogError("Cannot determine asset path at runtime.");
#endif

            url = null;
            return false;
        }
    }
    
    [Serializable]
    public class GraphicSource
    {
        public string id;
        public VectorImage graphic;
    }
    
    [Serializable]
    public class FontSource : FontDescriptor
    {
        public FontAsset font;
    }
}