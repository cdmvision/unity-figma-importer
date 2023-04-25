using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class AngularGradientPaint : GradientPaint
    {
        public override string type => PaintType.GradientAngular;
    }
}