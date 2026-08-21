using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class RadialGradientPaint : GradientPaint
    {
        public override PaintType type => PaintType.GradientRadial;
    }
}