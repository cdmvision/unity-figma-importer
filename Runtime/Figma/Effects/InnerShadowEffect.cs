using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class InnerShadowEffect : ShadowEffect
    {
        public override string type => EffectType.InnerShadow;
    }
}