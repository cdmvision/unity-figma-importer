using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A component set contains the variants of a component. It behaves much like a normal frame would, but all of
    /// its children are <see cref="ComponentNode"/>s.
    /// 
    /// In Figma, component sets must always have children. A component set with no children will delete itself.
    /// </summary>
    [DataContract]
    public class ComponentSetNode : FrameNode
    {
        public override string type => NodeType.ComponentSet;

        /// <summary>
        /// A mapping of name to <see cref="ComponentPropertyDefinition"/> for every component property on this
        /// component.
        /// </summary>
        [DataMember(Name = "componentPropertyDefinitions")]
        public Dictionary<string, ComponentPropertyDefinition> componentPropertyDefinitions { get; private set; } =
            new Dictionary<string, ComponentPropertyDefinition>();
        
        /// <summary>
        /// A list of component nodes that are children of this node.
        /// </summary>
        public ComponentNode[] variants => children.Cast<ComponentNode>().ToArray();
    }
}