using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FigmaComponentAttribute : Attribute
    {
        public string typeId { get; }

        public FigmaComponentAttribute(string typeId)
        {
            this.typeId = typeId;
        }
    }
}