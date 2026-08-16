using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentProperty
    {
        /// <summary>
        /// Type of this component property.
        /// </summary>
        [DataMember(Name = "type", IsRequired = true)]
        public virtual ComponentPropertyType type { get; set; }
    }

    [DataContract]
    public class ComponentPropertyInstanceSwap : ComponentProperty
    {
        public override ComponentPropertyType type => ComponentPropertyType.InstanceSwap;

        /// <summary>
        /// Value of this property set on this instance.
        /// </summary>
        [DataMember(Name = "value")]
        public string value { get; set; }
        
        /// <summary>
        /// List of user-defined preferred values for this property.
        /// </summary>
        [DataMember(Name = "preferredValues")]
        public InstanceSwapPreferredValue[] preferredValues { get; set; }
    }

    [DataContract]
    public class ComponentPropertyText : ComponentProperty
    {
        public override ComponentPropertyType type => ComponentPropertyType.Text;

        /// <summary>
        /// Value of this property set on this instance.
        /// </summary>
        [DataMember(Name = "value")]
        public string value { get; set; }
    }

    [DataContract]
    public class ComponentPropertyBoolean : ComponentProperty
    {
        public override ComponentPropertyType type => ComponentPropertyType.Boolean;

        /// <summary>
        /// Value of this property set on this instance.
        /// </summary>
        [DataMember(Name = "value")]
        public bool value { get; set; }
    }
    
    [DataContract]
    public enum InstanceSwapPreferredValueType
    {
        [EnumMember(Value = "COMPONENT")]
        Component,
        
        [EnumMember(Value = "COMPONENT_SET")]
        ComponentSet,
    }

    [DataContract]
    public class InstanceSwapPreferredValue
    {
        /// <summary>
        /// Type of node for this preferred value.
        /// </summary>
        [DataMember(Name = "type", IsRequired = true)]
        public InstanceSwapPreferredValueType type { get; set; }
        
        /// <summary>
        /// Key of this component or component set.
        /// </summary>
        [DataMember(Name = "key", IsRequired = true)]
        public string key { get; set; }
    }
}