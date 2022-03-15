using System;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyInt : StyleProperty<int>
    {
        public StylePropertyInt()
        {
        }

        public StylePropertyInt(int defaultValue) : base(defaultValue)
        {
        }
    }
}