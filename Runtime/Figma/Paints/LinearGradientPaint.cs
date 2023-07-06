using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class LinearGradientPaint : GradientPaint
    {
        public override string type => PaintType.GradientLinear;
    }
}