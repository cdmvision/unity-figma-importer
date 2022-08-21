using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FigmaNodeAttribute : Attribute
    {
        public string bind { get; set; }
    }
}