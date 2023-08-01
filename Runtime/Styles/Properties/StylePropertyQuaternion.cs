using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyQuaternion : StyleProperty<Quaternion>
    {
        public StylePropertyQuaternion()
        {
        }

        public StylePropertyQuaternion(Quaternion defaultValue) : base(defaultValue)
        {
        }
    }
}