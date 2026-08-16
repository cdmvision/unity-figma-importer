using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaResourceAttribute : Attribute
    {
        /// <summary>
        /// Path of the asset stored in a Resources folder.
        /// </summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Resources.html"/>
        public string path { get; }
        
        /// <summary>
        /// The binding of field or property is whether required.
        /// </summary>
        public bool isRequired { get; set; } = true;

        public FigmaResourceAttribute(string path)
        {
            this.path = path;
        }
    }
}