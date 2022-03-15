using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyBool : StyleProperty<bool>
    {
        public StylePropertyBool()
        {
        }

        public StylePropertyBool(bool defaultValue) : base(defaultValue)
        {
        }
    }
}