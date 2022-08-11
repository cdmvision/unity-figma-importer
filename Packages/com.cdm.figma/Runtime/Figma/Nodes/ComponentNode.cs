using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Components are UI elements that can be reused across your designs. They are like frames, with the additional
    /// ability to have auto-updating copies called instances (see <see cref="InstanceNode"/>).
    /// </summary>
    [DataContract]
    public class ComponentNode : FrameNode
    {
        public override string type => NodeType.Component;
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "componentProperties")]
        public ComponentProperties componentProperties { get; private set; }
        
        /// <summary>
        /// The component set that this component attached to.
        /// </summary>
        public ComponentSetNode componentSet { get; internal set; }
    }
}