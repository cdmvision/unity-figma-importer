using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class InnerShadowEffect : ShadowEffect
    {
        public override EffectType type => EffectType.InnerShadow;
    }
}