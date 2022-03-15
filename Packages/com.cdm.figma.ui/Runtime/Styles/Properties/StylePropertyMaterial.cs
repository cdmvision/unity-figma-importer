using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyMaterial: StyleProperty<Material>
    {
        public StylePropertyMaterial()
        {
        }

        public StylePropertyMaterial(Material defaultValue) : base(defaultValue)
        {
        }
    }
}