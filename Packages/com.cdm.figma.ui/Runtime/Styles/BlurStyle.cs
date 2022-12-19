using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public enum BlurType
    {
        Layer,
        Background
    }
    
    [Serializable]
    public class BlurStyle : StyleWithSetter<BlurStyleSetter>
    {
        public StylePropertyBool visible = new StylePropertyBool(true);
        public StylePropertyFloat radius = new StylePropertyFloat(4f);
        public StylePropertyBlurType type = new StylePropertyBlurType(BlurType.Layer);

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var blur = gameObject.GetComponent<BlurEffectBehaviour>();
            if (blur != null)
            {
                if (visible.enabled)
                    blur.enabled = visible.value;

                if (radius.enabled)
                    blur.radius = radius.value;

                if (type.enabled)
                    blur.type = type.value;
            }
        }
        
        protected override void MergeTo(Style other, bool force)
        {
            if (other is BlurStyle otherStyle)
            {
                OverwriteProperty(visible, otherStyle.visible, force);
                OverwriteProperty(radius, otherStyle.radius, force);
                OverwriteProperty(type, otherStyle.type, force);
            }
        }
    }
}