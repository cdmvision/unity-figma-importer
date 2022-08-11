using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentPropertyDefinition
    {
        [DataMember(Name = "type", IsRequired = true)]
        public virtual ComponentPropertyType type { get; set; }
    }

    [DataContract]
    public class ComponentPropertyDefinitionInstanceSwap : ComponentPropertyDefinition
    {
        public override ComponentPropertyType type => ComponentPropertyType.InstanceSwap;

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

        [DataMember(Name = "defaultValue")]
        public bool defaultValue { get; set; }
        
        public override string ToString()
        {
            return $"({type}, {defaultValue})";
        }
    }
}