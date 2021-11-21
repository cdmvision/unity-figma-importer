using System.Linq;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    public class DocumentNode : Node
    {
        public override NodeType type => NodeType.Document;
        
        /// <summary>
        /// A list of canvases attached to the document.
        /// </summary>
        [JsonProperty("children")]
        public CanvasNode[] children { get; set; }
        
        public override Node[] GetChildren() => children.Cast<Node>().ToArray();
    }
}