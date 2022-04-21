using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class ButtonComponentConverter : ComponentConverter<Button>
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
        
        public ButtonComponentConverter()
        {
            typeId = "Button";
            properties = new List<ComponentProperty>() { stateProperty };
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            if (SameVariant(variant, stateProperty.ToString(Default)))
            {
                selector = "";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Hover)))
            {
                selector = ":hover";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Press)))
            {
                selector = ":active";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Disabled)))
            {
                selector = ":disabled";
                return true;
            }

            selector = null;
            return false;
        }
    }
}