using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class NodeConverterHelper
    {
        public static void ConvertEffects(NodeObject nodeObject, IEnumerable<Effect> effects)
        {
            foreach (var effect in effects)
            {
                ConvertEffect(nodeObject, effect);
            }
        }
        
        public static bool ConvertEffect(NodeObject nodeObject, Effect effect)
        {
            if (effect is BlurEffect blurEffect)
            {
                return ConvertBlurEffect(nodeObject, blurEffect);
            }

            if (effect is ShadowEffect shadowEffect)
            {
                return ConvertShadowEffect(nodeObject, shadowEffect);
            }

            return false;
        }

        public static bool ConvertBlurEffect(NodeObject nodeObject, BlurEffect effect)
        {
            // TODO: Implement with LeTai Translucent Image plugin
            return false;
        }

        public static bool ConvertShadowEffect(NodeObject nodeObject, ShadowEffect effect)
        {
#if LETAI_TRUESHADOW
            var shadow = nodeObject.gameObject.AddComponent<LeTai.TrueShadow.TrueShadow>();
            shadow.enabled = effect.visible;
            shadow.Color = effect.color;
            shadow.Size = effect.radius;
            shadow.Inset = effect is InnerShadowEffect;

            var offset = (Vector2)effect.offset;
            shadow.OffsetDistance = offset.magnitude;
            shadow.OffsetAngle = Vector2.SignedAngle(Vector2.right, offset); // TODO: test

            if (effect.blendMode.HasValue)
            {
                switch (effect.blendMode.Value)
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
                            $"Shadow does not support {nameof(effect.blendMode.Value)} blend mode." +
                            $"{BlendMode.Normal} blend mode is used.");
                        break;
                }
            }

            return true;
#endif

            return false;
        }
    }
}