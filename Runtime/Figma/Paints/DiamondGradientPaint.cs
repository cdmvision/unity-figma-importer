using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class DiamondGradientPaint : GradientPaint
    {
        public override string type => PaintType.GradientDiamond;
    }
}