using System;

namespace Cdm.Figma.UI
{
    [AttributeUsage(AttributeTargets.Class |AttributeTargets.Field | AttributeTargets.Property)]
    public class FigmaNodeAttribute : Attribute
    {
        public string bind { get; }

        public FigmaNodeAttribute()
        {
        }
        
        public FigmaNodeAttribute(string bind)
        {
            this.bind = bind;
        }
    }
}