using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public abstract class ShadowEffectConverter : EffectConverter<ShadowEffect, ShadowStyle, ShadowEffectBehaviour>
    {
        protected override ShadowStyle CreateStyle(FigmaNode node, ShadowEffect effect)
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