using System;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyImageType : StyleProperty<Image.Type>
    {
        public StylePropertyImageType()
        {
        }

        public StylePropertyImageType(Image.Type defaultValue) : base(defaultValue)
        {
        }
    }
}