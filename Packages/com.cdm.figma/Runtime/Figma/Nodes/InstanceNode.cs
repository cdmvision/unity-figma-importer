using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Instances are a copy of a component (see <see cref="ComponentNode"/>). They will always be automatically
    /// updated if a component is modified.
    /// </summary>
    [DataContract]
    public class InstanceNode : FrameNode
    {
        public override string type => NodeType.Instance;
        
        /// <summary>
        /// ID of component that this instance came from, refers to components table.
        /// </summary>
        /// <seealso cref="mainComponent"/>
        [DataMember(Name = "componentId", IsRequired = true)]
        public string componentId { get; set; }
        
        /// <summary>
        /// The component that this instance reflects.
        /// </summary>
        public ComponentNode mainComponent { get; internal set; }
    }
}