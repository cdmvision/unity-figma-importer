using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyHorizontalAlignmentOptions : StyleProperty<HorizontalAlignmentOptions>
    {
        public StylePropertyHorizontalAlignmentOptions()
        {
        }

        public StylePropertyHorizontalAlignmentOptions(HorizontalAlignmentOptions defaultValue) : base(defaultValue)
        {
        }
    }
}