using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyFloat : StyleProperty<float>
    {
        public StylePropertyFloat()
        {
        }

        public StylePropertyFloat(float defaultValue) : base(defaultValue)
        {
        }
    }
}