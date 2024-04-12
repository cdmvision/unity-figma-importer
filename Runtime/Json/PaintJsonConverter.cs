using System;

namespace Cdm.Figma.Json
{
    public class PaintJsonConverter : SubTypeJsonConverter<Paint, PaintType>
    {
        protected override string GetTypeToken()
        {
            return nameof(Paint.type);
        }

        protected override bool TryGetActualType(PaintType typeToken, out Type type)
        {
            switch (typeToken)
            {
                case PaintType.Solid:
                    type = typeof(SolidPaint);
                    return true;
                case PaintType.GradientLinear:
                    type = typeof(LinearGradientPaint);
                    return true;
                case PaintType.GradientRadial:
                    type = typeof(RadialGradientPaint);
                    return true;
                case PaintType.GradientAngular:
                    type = typeof(AngularGradientPaint);
                    return true;
                case PaintType.GradientDiamond:
                    type = typeof(DiamondGradientPaint);
                    return true;
                case PaintType.Image:
                    type = typeof(ImagePaint);
                    return true;
                default:
                    type = typeof(Paint);
                    return true;
            }
        }
    }
}