using System;
using Cdm.Figma.UI.Styles.Properties;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    public class ImageStyle : StyleWithSelectors<ImageStylePropertyBlock>
    {
    }
    
    [Serializable]
    public class ImageStylePropertyBlock : GraphicStylePropertyBlock
    {
        public StylePropertySprite sprite = new StylePropertySprite();
        public StylePropertyImageType imageType = new StylePropertyImageType();
        public StylePropertyFloat pixelsPerUnitMultiplier = new StylePropertyFloat();
        
        public override void CopyTo(StylePropertyBlock other)
        {
            base.CopyTo(other);
            
            if (other is ImageStylePropertyBlock otherStyle)
            {
                sprite.CopyTo(otherStyle.sprite);
                imageType.CopyTo(otherStyle.imageType);
                pixelsPerUnitMultiplier.CopyTo(otherStyle.pixelsPerUnitMultiplier);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            base.SetStyle(gameObject, args);
            
            if (TryGetComponent<SVGImage>(gameObject, out var svgImage, false))
            {
                if (sprite.enabled)
                    svgImage.sprite = sprite.value;
            }
            else if (TryGetComponent<Image>(gameObject, out var image, false))
            {
                if (sprite.enabled)
                    image.sprite = sprite.value;

                if (imageType.enabled)
                {
                    image.type = imageType.value;
                    image.fillCenter = true;
                }

                if (pixelsPerUnitMultiplier.enabled)
                    image.pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
            }
            else
            {
                Debug.LogWarning($"Neither {nameof(SVGImage)} nor {nameof(Image)} component is found.", gameObject);
            }
        }
    }
}