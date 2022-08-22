using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class FigmaFileDependency
    {
        [DataMember(Name = "fileId", IsRequired = true)]
        public string fileId { get; set; }

        [DataMember(Name = "components")]
        public Dictionary<string, Component> components { get; set; }
            = new Dictionary<string, Component>();

        [DataMember(Name = "componentSets")]
        public Dictionary<string, ComponentSet> componentSets { get; set; }
            = new Dictionary<string, ComponentSet>();

        [DataMember(Name = "componentNodes")]
        public Dictionary<string, ComponentNode> componentNodes { get; set; }
            = new Dictionary<string, ComponentNode>();

        [DataMember(Name = "componentSetNodes")]
        public Dictionary<string, ComponentSetNode> componentSetNodes { get; set; }
            = new Dictionary<string, ComponentSetNode>();
    }
}