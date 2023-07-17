using System;

namespace Cdm.Figma.UI
{
    public class FigmaLocalizeAttribute : Attribute
    {
        public string localizationKey { get; }

        /// <summary>
        /// The binding of field or property is whether required.
        /// </summary>
        public bool isRequired { get; set; } = true;
        
        public FigmaLocalizeAttribute(string localizationKey)
        {
            this.localizationKey = localizationKey;
        }
    }
}