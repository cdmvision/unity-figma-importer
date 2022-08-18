using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class ImageStyle : GraphicStyle
    {
        public StylePropertyBool componentEnabled = new StylePropertyBool(true);
        public StylePropertySprite sprite = new StylePropertySprite();
        public StylePropertyImageType imageType = new StylePropertyImageType();
        public StylePropertyFloat pixelsPerUnitMultiplier = new StylePropertyFloat();
        
        protected override void MergeTo(Style other, bool force)
        {
            base.MergeTo(other, force);
            
            if (other is ImageStyle otherStyle)
            {
                OverwriteProperty(componentEnabled, otherStyle.componentEnabled, force);
                OverwriteProperty(sprite, otherStyle.sprite, force);
                OverwriteProperty(imageType, otherStyle.imageType, force);
                OverwriteProperty(pixelsPerUnitMultiplier, otherStyle.pixelsPerUnitMultiplier, force);
            }
        }

        public override void SetStyleAsSelector(GameObject gameObject, StyleArgs args)
        {
            base.SetStyleAsSelector(gameObject, args);
            
            SetStyleAsSelector<ImageStyleSetter>(gameObject, args);
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var image = gameObject.GetOrAddComponent<Image>();
            if (image != null)
            {
                base.SetStyle(gameObject, args);

                if (componentEnabled.enabled)
                    image.enabled = componentEnabled.value;
                
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