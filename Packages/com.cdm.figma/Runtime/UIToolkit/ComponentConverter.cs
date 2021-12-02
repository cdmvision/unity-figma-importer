using System.Collections.Generic;

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
        
        private readonly List<ComponentProperty> _properties = new List<ComponentProperty>();
        
        public IReadOnlyList<ComponentProperty> properties => _properties;

        public ComponentConverter()
        {
            typeId = GetDefaultTypeId();
            _properties.AddRange(GetVariants());
            
        }

        protected abstract string GetDefaultTypeId();
        protected abstract ISet<ComponentProperty> GetVariants();
        
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (node.GetType() != typeof(InstanceNode))
                return false;

            var componentType = node.GetComponentType();
            return !string.IsNullOrEmpty(componentType) && componentType == typeId;
        }
    }
}