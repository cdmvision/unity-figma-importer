using System.Collections.Generic;
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
        /// A mapping of name to <see cref="ComponentPropertyDefinition"/> for every component property on this
        /// component.
        /// </summary>
        [DataMember(Name = "componentPropertyDefinitions")]
        public Dictionary<string, ComponentPropertyDefinition> componentPropertyDefinitions { get; private set; } =
            new Dictionary<string, ComponentPropertyDefinition>();
        
        /// <summary>
        /// The component set that this component attached to.
        /// </summary>
        public ComponentSetNode componentSet { get; internal set; }
    }
}