using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyFloat : StyleProperty<float>
    {
        public StylePropertyFloat()
        {
        }

        public StylePropertyFloat(float defaultValue) : base(defaultValue)
        {
        }

        public override bool IsSameValue(StyleProperty<float> other)
        {
            return Mathf.Approximately(value, other.value);
        }
    }
}