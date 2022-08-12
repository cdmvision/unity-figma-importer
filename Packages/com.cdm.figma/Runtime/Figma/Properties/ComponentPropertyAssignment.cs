using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentPropertyAssignment
    {
        [DataMember(Name = "type", IsRequired = true)]
        public virtual ComponentPropertyType type { get; set; }
    }

    [DataContract]
    public class ComponentPropertyAssignmentInstanceSwap : ComponentPropertyAssignment
    {
        public override ComponentPropertyType type => ComponentPropertyType.InstanceSwap;

        [DataMember(Name = "value")]
        public string value { get; set; }
    }

    [DataContract]
    public class ComponentPropertyAssignmentText : ComponentPropertyAssignment
    {
        public override ComponentPropertyType type => ComponentPropertyType.Text;

        [DataMember(Name = "value")]
        public string value { get; set; }
    }

    [DataContract]
    public class ComponentPropertyAssignmentBoolean : ComponentPropertyAssignment
    {
        public override ComponentPropertyType type => ComponentPropertyType.Boolean;

        [DataMember(Name = "value")]
        public bool value { get; set; }
    }
}