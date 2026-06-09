

using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Utils
{
    public static class SpriteExtensions
    {
        public static Image.Type GetImageType(this Sprite sprite)
        {
            if (sprite == null)
                return Image.Type.Simple;
            
            return sprite.IsSliced() ? Image.Type.Sliced : Image.Type.Simple;
        }
    }
}