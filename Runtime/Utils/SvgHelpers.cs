using System;

namespace Cdm.Figma.Utils
{
    public static class SvgHelpers
    {
        private static class StrokeAlignment
        {
            public const string Center = "center";
            public const string Inner = "inner";
            public const string Outer = "outer";
        }

        public static string GetSvgValue(StrokeAlign? align)
        {
            if (!align.HasValue)
                return StrokeAlignment.Center;

            switch (align.Value)
            {
                case StrokeAlign.Center:
                    return "center";
                case StrokeAlign.Inside:
                    return "inner";
                case StrokeAlign.Outside:
                    return "outer";
                default:
                    throw new ArgumentOutOfRangeException(nameof(align), align, null);
            }
        }
    }
}