using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using JsonConverter = Unity.Plastic.Newtonsoft.Json.JsonConverter;

namespace Cdm.Figma
{
    /// <summary>
    /// The document referred to by :key as a JSON object. The file key can be parsed from
    /// any Figma file url: https://www.figma.com/file/:key/:title. The "document" attribute
    /// contains a node of type <see cref="NodeType.Document"/>.
    ///
    /// The "components" key contains a mapping from node IDs to component metadata. This is to
    /// help you determine which components each instance comes from. Currently the only piece of
    /// metadata available on components is the name of the component, but more properties will
    /// be forthcoming.
    /// </summary>
    [Serializable]
    public class FigmaFile
    {
        [JsonProperty("name")]
        public string name { get; set; }
        
        [JsonProperty("version")]
        public string version { get; set; }
        
        [JsonProperty("role")]
        public string role { get; set; }
        
        [JsonProperty("thumbnailUrl")]
        public string thumbnailUrl { get; set; }
        
        [JsonProperty("editorType")]
        public string editorType { get; set; }
        
        [JsonProperty("lastModified")]
        public DateTime lastModified { get; set; }

        /// <summary>
        /// The root node within the document.
        /// </summary>
        [JsonProperty("document")]
        public DocumentNode document { get; set; }
        
        /// <summary>
        /// The components key contains a mapping from node IDs to component metadata.
        /// This is to help you determine which components each instance comes from.
        /// </summary>
        [JsonProperty("components")]
        public Dictionary<string, Component> components { get; set; } = new Dictionary<string, Component>();
        
        [JsonProperty("componentSets")]
        public Dictionary<string, ComponentSet> componentSets { get; set; } = new Dictionary<string, ComponentSet>();
        
        [JsonProperty("styles")]
        public Dictionary<string, Style> styles { get; set; } = new Dictionary<string, Style>();
        
        [JsonProperty("schemaVersion")]
        public int schemaVersion { get; set; }

        public static FigmaFile FromString(string json) => 
            JsonConvert.DeserializeObject<FigmaFile>(json, Converter.Settings);

        public override string ToString()
        {
            return ToString("N");
        }
        
        public string ToString(string format)
        {
            if (string.IsNullOrEmpty(format)) format = "N";

            switch (format.ToUpperInvariant())
            {
                case "I":
                    return JsonConvert.SerializeObject(this, Formatting.Indented, Converter.Settings);
                case "N":
                    return JsonConvert.SerializeObject(this, Formatting.None, Converter.Settings);
                default:
                    throw new FormatException($"The {format} format string is not supported.");
            }
        }
    }
}