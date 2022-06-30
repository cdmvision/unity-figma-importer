using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class ImageStyle : GraphicStyle
    {
        public StylePropertySprite sprite = new StylePropertySprite();
        public StylePropertyImageType imageType = new StylePropertyImageType();
        public StylePropertyFloat pixelsPerUnitMultiplier = new StylePropertyFloat();

        public override void CopyTo(Style other)
        {
            base.CopyTo(other);

            if (other is ImageStyle otherStyle)
            {
                sprite.CopyTo(otherStyle.sprite);
                imageType.CopyTo(otherStyle.imageType);
                pixelsPerUnitMultiplier.CopyTo(otherStyle.pixelsPerUnitMultiplier);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var image = GetOrAddComponent<Image>(gameObject);
            if (image != null)
            {
                base.SetStyle(gameObject, args);
                
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
        }
    }
}