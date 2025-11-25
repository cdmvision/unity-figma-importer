using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyFontWeight : StyleProperty<FontWeight>
    {
        public StylePropertyFontWeight()
        {
        }

        public StylePropertyFontWeight(FontWeight defaultValue) : base(defaultValue)
        {
        }
    }
}