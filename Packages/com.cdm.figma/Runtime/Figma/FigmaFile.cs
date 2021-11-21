using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        public string name;

        /// <summary>
        /// A mapping from node IDs to component metadata. This is to help you determine which
        /// components each instance comes from. Currently the only piece of metadata available on
        /// components is the name of the component, but more properties will be forthcoming.
        /// </summary>
        [JsonProperty("components")]
        public Dictionary<string, Component> components { get; private set; } = new Dictionary<string, Component>();
        
        [JsonProperty("styles")]
        public Dictionary<string, Component> styles { get; private set; } = new Dictionary<string, Component>();

        /// <summary>
        /// The root node within the document
        /// </summary>
        [JsonProperty("document")]
        public RootNode document { get; set; }
        
        [JsonProperty("schemaVersion")]
        public float schemaVersion { get; set; }

        public static FigmaFile FromText(string json) => 
            JsonConvert.DeserializeObject<FigmaFile>(json, Converter.Settings);
    }
}