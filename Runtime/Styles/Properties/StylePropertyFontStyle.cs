using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyFontStyle : StyleProperty<FontStyles>
    {
        public StylePropertyFontStyle()
        {
        }

        public StylePropertyFontStyle(FontStyles defaultValue) : base(defaultValue)
        {
        }
    }
}