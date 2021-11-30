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
        /// The component that this instance reflects.
        /// </summary>
        public ComponentSetNode componentSet { get; internal set; }
    }
}