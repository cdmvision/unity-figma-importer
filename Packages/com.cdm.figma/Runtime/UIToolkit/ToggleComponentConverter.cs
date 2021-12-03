using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public static class ToggleState
    {
        public const string On = "On";
        public const string Off = "Off";
    }
    
    public class ToggleComponentConverter : ComponentConverter<Toggle>
    {
        public ToggleComponentConverter()
        {
            typeId = "Toggle";
            properties = new List<ComponentProperty>()
            {
                new ComponentProperty()
                {
                    key = "State",
                    variants = new ComponentVariant[]
                    {
                        new ComponentVariant(ComponentState.Default, ComponentState.Default),
                        new ComponentVariant(ComponentState.Hover, ComponentState.Hover),
                        new ComponentVariant(ComponentState.Press, ComponentState.Press),
                        new ComponentVariant(ComponentState.Disabled, ComponentState.Disabled),
                    }
                },
                new ComponentProperty()
                {
                    key = "Checked",
                    variants = new ComponentVariant[]
                    {
                        new ComponentVariant(ToggleState.On, ToggleState.On),
                        new ComponentVariant(ToggleState.Off, ToggleState.Off),
                    }
                }
            };
        }
    }
}