using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class RadialGradientPaint : GradientPaint
    {
        public override string type => PaintType.GradientRadial;
    }
}