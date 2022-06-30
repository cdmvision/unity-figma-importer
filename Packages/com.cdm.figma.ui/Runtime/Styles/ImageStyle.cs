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

        public override bool SetStyle(GameObject gameObject, StyleArgs args)
        {
            if (!base.SetStyle(gameObject, args))
                return false;
            
            if (TryGetComponent<SVGImage>(gameObject, out var svgImage, false))
            {
                if (sprite.enabled)
                    svgImage.sprite = sprite.value;
                return true;
            }
            
            if (TryGetComponent<Image>(gameObject, out var image, false))
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
                return true;
            }
            
            Debug.LogWarning($"Neither {nameof(SVGImage)} nor {nameof(Image)} component is found.", gameObject);
            return false;
        }
    }
}