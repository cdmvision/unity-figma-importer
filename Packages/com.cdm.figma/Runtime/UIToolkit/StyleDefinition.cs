using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class StyleDefinition
    {
        public string selector { get; }
        public Style style { get; }

        public StyleDefinition(string selector)
        {
            this.selector = selector;
            style = new Style();
        }
        
        public StyleDefinition(string selector, Style style)
        {
            this.selector = selector;
            this.style = style;
        }
    }
}