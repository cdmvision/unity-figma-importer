using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaNodeAttribute : FigmaImporterAttribute
    {
        public string bindingKey { get; }
        
        /// <summary>
        /// The field or property is whether required. No effect on a class.
        /// </summary>
        public bool isRequired { get; set; }

        public FigmaNodeAttribute()
        {
        }

        public FigmaNodeAttribute(string bindingKey)
        {
            this.bindingKey = bindingKey;
        }
    }
}