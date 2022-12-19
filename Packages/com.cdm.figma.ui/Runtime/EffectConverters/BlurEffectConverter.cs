using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public abstract class BlurEffectConverter : EffectConverter<BlurEffect, BlurStyle, BlurEffectBehaviour>
    {
        protected override BlurStyle CreateStyle(FigmaNode node, BlurEffect effect)
        {
            var style = new BlurStyle();
            style.enabled = effect.visible;
            style.radius.SetValue(effect.radius);
            style.type.SetValue(effect is BackgroundBlurEffect ? BlurType.Background : BlurType.Layer);
            return style;
        }
    }
}