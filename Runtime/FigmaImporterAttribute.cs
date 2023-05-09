using System;

namespace Cdm.Figma.UI
{
    public class FigmaImporterAttribute : Attribute
    {
        public string importerExtension { get; set; } = "figma";

        public FigmaImporterAttribute()
        {
        }
    }
}