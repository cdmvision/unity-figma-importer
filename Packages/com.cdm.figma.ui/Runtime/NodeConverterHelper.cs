using System.Collections.Generic;
using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public static class NodeConverterHelper
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

        public static void GenerateBlurEffectStyle(FigmaNode nodeObject, BlurEffect effect)
        {
            // TODO: Implement with LeTai Translucent Image plugin
        }

        public static void GenerateShadowEffectStyle(FigmaNode nodeObject, ShadowEffect effect)
        {
            var style = new ShadowStyle();
            style.enabled = effect.visible;
            
            style.color.SetValue(effect.color);
            style.radius.SetValue(effect.radius);
            style.inner.SetValue(effect is InnerShadowEffect);
            style.offset.SetValue(effect.offset);
            style.spread.SetValue(effect.spread);

            if (effect.blendMode.HasValue)
            {
                style.blendMode.SetValue(effect.blendMode.Value);
            }
            
            nodeObject.styles.Add(style);
        }
    }
}