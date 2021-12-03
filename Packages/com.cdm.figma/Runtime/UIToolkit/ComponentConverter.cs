using System.Collections.Generic;
using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public static class ComponentState
    {
        public const string Default = "Default";
        public const string Hover = "Hover";
        public const string Press = "Press";
        public const string Disabled = "Disabled";
        public const string Selected = "Selected";
    }
    
    public abstract class ComponentConverter : NodeConverter
    {
        public string typeId { get; set; }
        
        private List<ComponentProperty> _properties = new List<ComponentProperty>();

        public List<ComponentProperty> properties
        {
            get => _properties;
            set => _properties = value ?? new List<ComponentProperty>();
        }

        public ComponentConverter()
        {
        }
        
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (node.GetType() != typeof(InstanceNode))
                return false;

            var componentType = node.GetComponentType();
            return !string.IsNullOrEmpty(componentType) && componentType == typeId;
        }
    }

    public abstract class ComponentConverter<TComponent> : ComponentConverter
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return XmlFactory.NewElement<TComponent>(node, args);
        }
    }
}