using Cdm.Figma.UI.Styles;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Utils
{
    public static class ImageStyleExtensions
    {
        public static void SetSpriteWithStyles(this Image image, Sprite sprite)
        {
            var setter = image.GetComponent<ImageStyleSetter>();
            image.sprite = sprite;
            
            if(setter != null)
            {
                foreach (var style in setter.styles)
                {
                    style.sprite.value = sprite;
                }
            }
        }
    }
}