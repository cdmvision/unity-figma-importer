using System;

namespace Cdm.Figma.Utils
{
    public class EffectJsonConverter : SubTypeJsonConverter<Effect, EffectType>
    {
        protected override string GetTypeToken()
        {
            return nameof(Effect.type);
        }

        protected override bool TryGetActualType(EffectType typeToken, out Type type)
        {
            switch (typeToken)
            {
                case EffectType.InnerShadow:
                    type = typeof(InnerShadowEffect);
                    return true;
                case EffectType.DropShadow:
                    type = typeof(DropShadowEffect);
                    return true;
                case EffectType.LayerBlur:
                    type = typeof(LayerBlurEffect);
                    return true;
                case EffectType.BackgroundBlur:
                    type = typeof(BackgroundBlurEffect);
                    return true;
                default:
                    type = typeof(Effect);
                    return true;
            }
        }
    }
}