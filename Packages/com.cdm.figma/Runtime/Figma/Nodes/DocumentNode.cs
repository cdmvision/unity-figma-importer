using System.Linq;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The document node is the root node. There can only be one document node per file, and each of its children
    /// must be a <see cref="PageNode"/>.
    /// </summary>
    [DataContract]
    public class DocumentNode : Node
    {
        public override string type => NodeType.Document;
        
        /// <summary>
        /// A list of canvases attached to the document.
        /// </summary>
        [DataMember(Name = "children")]
        public PageNode[] children { get; set; }
        
        public override Node[] GetChildren() => children.Cast<Node>().ToArray();
    }
}