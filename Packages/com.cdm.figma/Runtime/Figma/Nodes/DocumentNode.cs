using System.Linq;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class DocumentNode : Node
    {
        public override string type => NodeType.Document;
        
        /// <summary>
        /// A list of canvases attached to the document.
        /// </summary>
        [DataMember(Name = "children")]
        public CanvasNode[] children { get; set; }
        
        public override Node[] GetChildren() => children.Cast<Node>().ToArray();
    }
}