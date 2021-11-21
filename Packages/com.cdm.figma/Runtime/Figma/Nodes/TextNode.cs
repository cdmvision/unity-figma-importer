using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class TextNode : VectorNode
    {
        public override NodeType type => NodeType.Text;
        
        /// <summary>
        /// Text contained within a text box.
        /// </summary>
        [JsonProperty("characters")]
        public string characters { get; set; }
        
        /// <summary>
        /// Style of text including font family and weight.
        /// </summary>
        [JsonProperty("style")]
        public TypeStyle style { get; set; }
        
        /// <summary>
        /// Array with same number of elements as characters in text box, each element is a reference to the
        /// <see cref="styleOverrideTable"/> defined below and maps to the corresponding character in the characters
        /// field. Elements with value 0 have the default type style.
        /// </summary>
        [JsonProperty("characterStyleOverrides")]
        public int[] characterStyleOverrides { get; set; }

        /// <summary>
        /// Map from ID to <see cref="TypeStyle"/> for looking up style overrides.
        /// </summary>
        [JsonProperty("styleOverrideTable")]
        public Dictionary<int, TypeStyle> styleOverrideTable { get; private set; } = new Dictionary<int, TypeStyle>();
    }
}