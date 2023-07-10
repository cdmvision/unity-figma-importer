using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyBlurType : StyleProperty<BlurType>
    {
        public StylePropertyBlurType()
        {
        }

        public StylePropertyBlurType(BlurType defaultValue) : base(defaultValue)
        {
        }
    }
}