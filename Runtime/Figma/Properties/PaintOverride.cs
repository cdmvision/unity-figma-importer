using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class PaintOverride
    {
        /// <summary>
        /// An array of fill paints applied to the node.
        /// </summary>
        [DataMember(Name = "fills")]
        public List<Paint> fills { get; private set; } = new List<Paint>();
        
        /// <summary>
        /// ID of style node, if any, that this inherits fill data from.
        /// </summary>
        [DataMember(Name = "inheritFillStyleId")]
        public string inheritFillStyleId { get; set; }
    }
}