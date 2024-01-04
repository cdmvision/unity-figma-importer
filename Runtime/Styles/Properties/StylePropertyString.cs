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

        public override bool IsSameValue(StyleProperty<string> other)
        {
            return value.Equals(other.value, StringComparison.Ordinal);
        }
    }
}