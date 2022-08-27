using System.Collections.Generic;
using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public static class StyleGeneratorHelper
    {
        public static void GenerateEffectsStyles(FigmaNode figmaNode, IEnumerable<Effect> effects)
        {
            foreach (var effect in effects)
            {
                var style = GenerateEffectStyle(effect);
                if (style != null)
                {
                    figmaNode.styles.Add(style);
                }
            }
        }

        public static Styles.Style GenerateEffectStyle(Effect effect)
        {
            Styles.Style style = null;

            switch (effect)
            {
                case BlurEffect blurEffect:
                    style = GenerateBlurEffectStyle(blurEffect);
                    break;
                case ShadowEffect shadowEffect:
                    style = GenerateShadowEffectStyle(shadowEffect);
                    break;
            }

            return style;
        }

        public static Styles.Style GenerateBlurEffectStyle(BlurEffect effect)
        {
            var style = new BlurStyle();
            style.enabled = effect.visible;
            style.radius.SetValue(effect.radius);
            style.type.SetValue(effect is BackgroundBlurEffect ? BlurType.Background : BlurType.Layer);
            return style;
        }

        public static Styles.Style GenerateShadowEffectStyle(ShadowEffect effect)
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

            return style;
        }
    }
}