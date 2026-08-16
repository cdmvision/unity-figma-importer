using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyTextOverflowMode : StyleProperty<TextOverflowModes>
    {
        public StylePropertyTextOverflowMode()
        {
        }

        public StylePropertyTextOverflowMode(TextOverflowModes defaultValue) : base(defaultValue)
        {
        }
    }
}