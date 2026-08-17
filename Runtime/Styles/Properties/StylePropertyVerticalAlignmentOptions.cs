using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{    
    [Serializable]
    public class StylePropertyVerticalAlignmentOptions : StyleProperty<VerticalAlignmentOptions>
    {
        public StylePropertyVerticalAlignmentOptions()
        {
        }

        public StylePropertyVerticalAlignmentOptions(VerticalAlignmentOptions defaultValue) : base(defaultValue)
        {
        }
    }
}