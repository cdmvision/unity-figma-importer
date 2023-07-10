using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyTextAlignment : StyleProperty<TextAlignmentOptions>
    {
        public StylePropertyTextAlignment()
        {
        }

        public StylePropertyTextAlignment(TextAlignmentOptions defaultValue) : base(defaultValue)
        {
        }
    }
}