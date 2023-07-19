using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentPropertyDefinition
    {
        /// <summary>
        /// Type of this component property.
        /// </summary>
        [DataMember(Name = "type", IsRequired = true)]
        public virtual ComponentPropertyType type { get; set; }
    }

    [DataContract]
    public class ComponentPropertyDefinitionInstanceSwap : ComponentPropertyDefinition
    {
        public override ComponentPropertyType type => ComponentPropertyType.InstanceSwap;

        /// <summary>
        /// Initial value of this property for instances.
        /// </summary>
        [DataMember(Name = "defaultValue")]
        public string defaultValue { get; set; }

        public override string ToString()
        {
            return $"({type}, {defaultValue})";
        }
    }

    [DataContract]
    public class ComponentPropertyDefinitionText : ComponentPropertyDefinition
    {
        public override ComponentPropertyType type => ComponentPropertyType.Text;

        /// <summary>
        /// Initial value of this property for instances.
        /// </summary>
        [DataMember(Name = "defaultValue")]
        public string defaultValue { get; set; }

        public override string ToString()
        {
            return $"({type}, {defaultValue})";
        }
    }

    [DataContract]
    public class ComponentPropertyDefinitionBoolean : ComponentPropertyDefinition
    {
        public override ComponentPropertyType type => ComponentPropertyType.Boolean;

        /// <summary>
        /// Initial value of this property for instances.
        /// </summary>
        [DataMember(Name = "defaultValue")]
        public bool defaultValue { get; set; }

        public override string ToString()
        {
            return $"({type}, {defaultValue})";
        }
    }
    
    [DataContract]
    public class ComponentPropertyDefinitionVariant : ComponentPropertyDefinition
    {
        public override ComponentPropertyType type => ComponentPropertyType.Variant;

        /// <summary>
        /// Initial value of this property for instances.
        /// </summary>
        [DataMember(Name = "defaultValue")]
        public string defaultValue { get; set; }
        
        /// <summary>
        /// All possible values for this property.
        /// </summary>
        [DataMember(Name = "variantOptions")]
        public string[] variantOptions { get; set; }

        public override string ToString()
        {
            return $"({type}, {defaultValue})";
        }
    }
}