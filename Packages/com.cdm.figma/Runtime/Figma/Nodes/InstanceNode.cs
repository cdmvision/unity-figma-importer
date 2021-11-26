using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class InstanceNode : FrameNode
    {
        public override string type => NodeType.Instance;
        
        /// <summary>
        /// ID of component that this instance came from, refers to components table.
        /// </summary>
        [DataMember(Name = "componentId", IsRequired = true)]
        public string componentId { get; set; }
    }
}