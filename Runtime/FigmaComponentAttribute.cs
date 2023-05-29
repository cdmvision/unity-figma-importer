using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FigmaComponentAttribute : FigmaImporterAttribute
    {
        public string typeId { get; }

        public FigmaComponentAttribute(string typeId)
        {
            this.typeId = typeId;
        }
    }
}