using System.Collections.Generic;
using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public static class StyleGeneratorHelper
    {
        public static void GenerateEffectsStyles(FigmaNode nodeObject, IEnumerable<Effect> effects)
        {
            foreach (var effect in effects)
            {
                GenerateEffectStyle(nodeObject, effect);
            }
        }

        public static void GenerateEffectStyle(FigmaNode nodeObject, Effect effect)
        {
            switch (effect)
            {
                case BlurEffect blurEffect:
                    GenerateBlurEffectStyle(nodeObject, blurEffect);
                    break;
                case ShadowEffect shadowEffect:
                    GenerateShadowEffectStyle(nodeObject, shadowEffect);
                    break;
            }
        }

        public static void GenerateBlurEffectStyle(FigmaNode figmaNode, BlurEffect effect)
        {
            var style = new BlurStyle();
            style.enabled = effect.visible;
            style.radius.SetValue(effect.radius);
            style.type.SetValue(effect is BackgroundBlurEffect ? BlurType.Background : BlurType.Layer);

            figmaNode.styles.Add(style);
        }

        public static void GenerateShadowEffectStyle(FigmaNode figmaNode, ShadowEffect effect)
        {
            var style = new ShadowStyle();
            style.enabled = effect.visible;

            style.color.SetValue(effect.color);
            style.radius.SetValue(effect.radius);
            style.inner.SetValue(effect is InnerShadowEffect);
            style.offset.SetValue(effect.offset);

            if (effect.spread > 0)
            {
                style.spread.SetValue(effect.spread);
            }

            if (effect.blendMode.HasValue)
            {
                style.blendMode.SetValue(effect.blendMode.Value);
            }

            figmaNode.styles.Add(style);
        }
    }
}