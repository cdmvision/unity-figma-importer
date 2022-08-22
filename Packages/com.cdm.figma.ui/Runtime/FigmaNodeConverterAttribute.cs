using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FigmaNodeConverterAttribute : Attribute
    {
        public string importerExtension { get; }

        public FigmaNodeConverterAttribute()
        {
            this.importerExtension = "figma";
        }
        
        public FigmaNodeConverterAttribute(string importerExtension)
        {
            this.importerExtension = importerExtension;
        }
    }
}