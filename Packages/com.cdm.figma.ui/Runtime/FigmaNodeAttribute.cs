using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaNodeAttribute : FigmaImporterAttribute
    {
        /// <summary>
        /// The binding key set by the user in Figma editor using Unity Figma Plugin.
        /// </summary>
        public string bindingKey { get; }

        /// <summary>
        /// The field or property is whether required. No effect on a class.
        /// </summary>
        public bool isRequired { get; set; } = true;

        public FigmaNodeAttribute()
        {
        }

        public FigmaNodeAttribute(string bindingKey)
        {
            this.bindingKey = bindingKey;
        }
    }
}