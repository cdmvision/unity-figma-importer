using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyString : StyleProperty<string>
    {
        public StylePropertyString()
        {
        }

        public StylePropertyString(string defaultValue) : base(defaultValue)
        {
        }
    }
}