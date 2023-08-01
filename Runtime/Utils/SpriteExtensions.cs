using UnityEngine;

namespace Cdm.Figma.Utils
{
    public static class SpriteExtensions
    {
        public static bool IsSliced(this Sprite sprite)
        {
            return sprite.border != Vector4.zero;
        }
    }
}