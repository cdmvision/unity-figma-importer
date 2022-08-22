using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FigmaComponentAttribute : Attribute
    {
        public string importerExtension { get; }
        public string typeId { get; }        
        
        public FigmaComponentAttribute(string typeId)
        {
            this.typeId = typeId;
            this.importerExtension = "figma";
        }
        
        public FigmaComponentAttribute(string typeId, string importerExtension)
        {
            this.typeId = typeId;
            this.importerExtension = importerExtension;
        }
    }
}