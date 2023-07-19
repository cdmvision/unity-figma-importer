using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public abstract class StyleWithSetter<T> : Style where T : StyleSetterWithSelectorsBase
    {
        public override void SetStyleAsSelector(GameObject gameObject, StyleArgs args)
        {
            SetStyleAsSelector<T>(gameObject, args);
        }
    }
}