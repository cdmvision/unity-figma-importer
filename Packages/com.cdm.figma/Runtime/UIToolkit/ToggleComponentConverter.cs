using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public static class ToggleState
    {
        public const string On = "On";
        public const string Off = "Off";
    }
    
    public class ToggleComponentConverter : ComponentConverter
    {
        protected override string GetDefaultTypeId()
        {
            return "Toggle";
        }
        
        protected override ISet<ComponentProperty> GetVariants()
        {
            return new HashSet<ComponentProperty>()
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

        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return XmlFactory.NewElement<Toggle>(node, args);
        }
    }
}