using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class LayerBlurEffect : BlurEffect
    {
        public override EffectType type => EffectType.LayerBlur;
    }
}