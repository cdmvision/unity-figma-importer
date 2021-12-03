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
        /// A list of component nodes that are children of this node.
        /// </summary>
        public ComponentNode[] components => children.Cast<ComponentNode>().ToArray();
    }
}