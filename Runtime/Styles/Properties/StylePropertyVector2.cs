using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyVector2 : StyleProperty<Vector2>
    {
        public StylePropertyVector2()
        {
        }

        public StylePropertyVector2(Vector2 defaultValue) : base(defaultValue)
        {
        }
    }
}