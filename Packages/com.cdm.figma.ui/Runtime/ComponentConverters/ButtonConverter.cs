using System.Collections.Generic;
using Cdm.Figma.UI.Styles;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ButtonConverter : ComponentConverter<Button>
    {
        private const int Default = 0;
        private const int Hover = 1;
        private const int Press = 2;
        private const int Disabled = 3;
        
        public ComponentProperty stateProperty { get; } = new ComponentProperty("State", new[]
        {
            ComponentState.Default,
            ComponentState.Hover,
            ComponentState.Press,
            ComponentState.Disabled,
        });
        
        public ButtonConverter()
        {
            typeId = "Button";
            properties = new List<ComponentProperty>() { stateProperty };
        }

        protected override bool TryGetSelector(string[] variant, out Selector selector)
        {
            if (IsSameVariant(variant, stateProperty.ToString(Default)))
            {
                selector = Selector.Normal;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Hover)))
            {
                selector = Selector.Highlighted;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Press)))
            {
                selector = Selector.Pressed;
                return true;
            }

            if (IsSameVariant(variant, stateProperty.ToString(Disabled)))
            {
                selector = Selector.Disabled;
                return true;
            }

            selector = Selector.Normal;
            return false;
        }
    }
}