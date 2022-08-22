using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FigmaComponentConverterAttribute : FigmaNodeConverterAttribute
    {
        public FigmaComponentConverterAttribute()
        {
        }

        public FigmaComponentConverterAttribute(string importerExtension) : base(importerExtension)
        {
        }
    }
}