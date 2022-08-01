using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class ShadowStyle : Style
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

        public override void SetStyleAsSelector(GameObject gameObject, StyleArgs args)
        {
            SetStyleAsSelector<ShadowStyleSetter>(gameObject, args);
        }
        
        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
#if LETAI_TRUESHADOW
            var shadow = GetOrAddComponent<LeTai.TrueShadow.TrueShadow>(gameObject);
            if (shadow != null)
            {
                shadow.UseCasterAlpha = true;
                shadow.IgnoreCasterColor = true;

                if (visible.enabled)
                    shadow.enabled = visible.value;

                if (inner.enabled)
                    shadow.Inset = inner.value;

                if (color.enabled)
                    shadow.Color = color.value;

                if (radius.enabled)
                    shadow.Size = radius.value;

                if (spread.enabled)
                    Debug.LogWarning($"{nameof(ShadowStyle)} does not support {nameof(spread)} property.");


                if (offset.enabled)
                {
                    shadow.OffsetDistance = offset.value.magnitude;
                    shadow.OffsetAngle = Vector2.SignedAngle(Vector2.right, offset.value); // TODO: test
                }

                if (blendMode.enabled)
                {
                    switch (blendMode.value)
                    {
                        case BlendMode.ColorDodge:
                            shadow.BlendMode = LeTai.TrueShadow.BlendMode.Additive;
                            break;
                        case BlendMode.Multiply:
                            shadow.BlendMode = LeTai.TrueShadow.BlendMode.Multiply;
                            break;
                        case BlendMode.Normal:
                            shadow.BlendMode = LeTai.TrueShadow.BlendMode.Normal;
                            break;
                        case BlendMode.Screen:
                            shadow.BlendMode = LeTai.TrueShadow.BlendMode.Screen;
                            break;
                        default:
                            shadow.BlendMode = LeTai.TrueShadow.BlendMode.Normal;

                            Debug.LogWarning(
                                $"{nameof(ShadowStyle)} does not support {nameof(blendMode.value)} blend mode." +
                                $"{BlendMode.Normal} blend mode is used.");
                            break;
                    }
                }
            }
#endif
        }
    }
}