using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaAssetAttribute : FigmaImporterAttribute
    {
        /// <summary>
        /// Optional display name of the asset.
        /// </summary>
        public string name { get; set; }
        
        /// <summary>
        /// The binding of field or property is whether required.
        /// </summary>
        public bool isRequired { get; set; } = true;

        public FigmaAssetAttribute()
        {
        }
    }
}