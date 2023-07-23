using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyBlendMode : StyleProperty<BlendMode>
    {
        public StylePropertyBlendMode()
        {
        }

        public StylePropertyBlendMode(BlendMode defaultValue) : base(defaultValue)
        {
        }
    }
}