using System;
using Cdm.Figma.UI.Effects;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class ShadowStyle : StyleWithSetter<ShadowStyleSetter>
    {
        public StylePropertyBool visible = new StylePropertyBool(true);
        public StylePropertyBool inner = new StylePropertyBool(false);
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.black);
        public StylePropertyFloat radius = new StylePropertyFloat(4f);
        public StylePropertyFloat spread = new StylePropertyFloat(4f);
        public StylePropertyVector2 offset = new StylePropertyVector2(Vector2.zero);
        public StylePropertyBlendMode blendMode = new StylePropertyBlendMode(BlendMode.Normal);

        protected override void MergeTo(Style other, bool force)
        {
            if (other is ShadowStyle otherStyle)
            {
                OverwriteProperty(visible, otherStyle.visible, force);
                OverwriteProperty(inner, otherStyle.inner, force);
                OverwriteProperty(color, otherStyle.color, force);
                OverwriteProperty(radius, otherStyle.radius, force);
                OverwriteProperty(spread, otherStyle.spread, force);
                OverwriteProperty(offset, otherStyle.offset, force);
                OverwriteProperty(blendMode, otherStyle.blendMode, force);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var shadow = gameObject.GetComponent<Shadow>();
            if (shadow != null)
            {
                if (visible.enabled)
                    shadow.enabled = visible.value;

                if (inner.enabled)
                    shadow.inner = inner.value;

                if (color.enabled)
                    shadow.color = color.value;

                if (radius.enabled)
                    shadow.radius = radius.value;

                if (spread.enabled)
                    shadow.spread = spread.value;

                if (offset.enabled)
                    shadow.offset = offset.value;

                if (blendMode.enabled)
                    shadow.blendMode = blendMode.value;
            }
        }
    }
}