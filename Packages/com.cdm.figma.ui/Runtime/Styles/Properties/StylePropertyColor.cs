using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyColor : StyleProperty<UnityEngine.Color>
    {
        public StylePropertyColor()
        {
        }

        public StylePropertyColor(UnityEngine.Color defaultValue) : base(defaultValue)
        {
        }
    }
}