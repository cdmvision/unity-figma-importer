using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyTexture : StyleProperty<Texture>
    {
        public StylePropertyTexture()
        {
        }

        public StylePropertyTexture(Texture defaultValue) : base(defaultValue)
        {
        }
    }
}