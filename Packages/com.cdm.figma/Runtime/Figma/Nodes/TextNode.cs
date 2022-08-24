using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The text node represents text where both the whole node or individual character ranges can have properties
    /// such as color (fills), font size, font name, etc.
    /// </summary>
    [DataContract]
    public class TextNode : VectorNode
    {
        public override string type => NodeType.Text;
        
        /// <summary>
        /// Text contained within a text box.
        /// </summary>
        [DataMember(Name = "characters")]
        public string characters { get; set; }
        
        /// <summary>
        /// Style of text including font family and weight.
        /// </summary>
        [DataMember(Name = "style")]
        public TypeStyle style { get; set; }
        
        /// <summary>
        /// Array with same number of elements as characters in text box, each element is a reference to the
        /// <see cref="styleOverrideTable"/> defined below and maps to the corresponding character in the characters
        /// field. Elements with value 0 have the default type style.
        /// </summary>
        [DataMember(Name = "characterStyleOverrides")]
        public int[] characterStyleOverrides { get; set; }

        /// <summary>
        /// Map from ID to <see cref="TypeStyle"/> for looking up style overrides.
        /// </summary>
        [DataMember(Name = "styleOverrideTable")]
        public Dictionary<int, TypeStyle> styleOverrideTable { get; private set; } = new Dictionary<int, TypeStyle>();
        
        /// <summary>
        /// <see cref="ComponentProperties.references"/> will be available if text node located inside
        /// <see cref="ComponentSetNode"/>'s variant component.
        /// </summary>
        [DataMember(Name = "componentProperties")]
        public ComponentProperties componentProperties { get; private set; }
    }
}