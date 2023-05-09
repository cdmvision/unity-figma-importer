using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaResourceAttribute : Attribute
    {
        public string path { get; }
        
        /// <summary>
        /// The field or property is whether required.
        /// </summary>
        public bool isRequired { get; set; }

        public FigmaResourceAttribute(string path)
        {
            this.path = path;
        }
    }
}