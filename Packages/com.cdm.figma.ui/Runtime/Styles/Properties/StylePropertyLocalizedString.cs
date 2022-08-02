using System;
using UnityEngine.Localization;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyLocalizedString : StyleProperty<LocalizedString>
    {
        public StylePropertyLocalizedString()
        {
        }

        public StylePropertyLocalizedString(LocalizedString defaultValue) : base(defaultValue)
        {
        }
    }
}