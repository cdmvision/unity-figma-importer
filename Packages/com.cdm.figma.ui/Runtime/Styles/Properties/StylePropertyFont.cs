using System;
using TMPro;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyFont : StyleProperty<TMP_FontAsset>
    {
        public StylePropertyFont()
        {
        }

        public StylePropertyFont(TMP_FontAsset defaultValue) : base(defaultValue)
        {
        }
    }
}