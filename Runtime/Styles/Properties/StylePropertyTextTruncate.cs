using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyTextTruncate : StyleProperty<TextOverflowModes>
    {
        public StylePropertyTextTruncate()
        {
        }

        public StylePropertyTextTruncate(TextOverflowModes defaultValue) : base(defaultValue)
        {
        }
    }
}