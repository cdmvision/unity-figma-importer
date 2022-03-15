using System;
using System.Numerics;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertyVector3 : StyleProperty<Vector3>
    {
        public StylePropertyVector3()
        {
        }

        public StylePropertyVector3(Vector3 defaultValue) : base(defaultValue)
        {
        }
    }
}