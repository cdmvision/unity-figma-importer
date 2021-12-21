﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using UnityEditor.Experimental.GraphView;

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
    [DataContract]
    public class FigmaFileContent
    {
        [DataMember(Name = "name")]
        public string name { get; set; }
        
        [DataMember(Name = "version")]
        public string version { get; set; }
        
        [DataMember(Name = "role")]
        public string role { get; set; }
        
        [DataMember(Name = "thumbnailUrl")]
        public string thumbnailUrl { get; set; }
        
        [DataMember(Name = "editorType")]
        public string editorType { get; set; }
        
        [DataMember(Name = "lastModified")]
        public DateTime lastModified { get; set; }

        /// <summary>
        /// The root node within the document.
        /// </summary>
        [DataMember(Name = "document")]
        public DocumentNode document { get; set; }
        
        /// <summary>
        /// Data written by plugins that is visible only to the plugin that wrote it. Requires the `pluginData` to
        /// include the ID of the plugin.
        /// </summary>
        [DataMember(Name = "pluginData")]
        public Dictionary<string, JObject> pluginData { get; set; }
        
        /// <summary>
        /// Data written by plugins that is visible to all plugins. Requires the `pluginData` parameter to include
        /// the string "shared".
        /// </summary>
        [DataMember(Name = "sharedPluginData")]
        public Dictionary<string, JObject> sharedPluginData { get; set; }
        
        /// <summary>
        /// The components key contains a mapping from node IDs to component metadata.
        /// This is to help you determine which components each instance comes from.
        /// </summary>
        [DataMember(Name = "components")]
        public Dictionary<string, Component> components { get; set; } = new Dictionary<string, Component>();
        
        [DataMember(Name = "componentSets")]
        public Dictionary<string, ComponentSet> componentSets { get; set; } = new Dictionary<string, ComponentSet>();
        
        [DataMember(Name = "styles")]
        public Dictionary<string, Style> styles { get; set; } = new Dictionary<string, Style>();
        
        [DataMember(Name = "schemaVersion")]
        public int schemaVersion { get; set; }

        public void BuildHierarchy()
        {
            InitComponentNodes();
            InitInstanceNodes();
            
            BuildHierarchy(document);
        }

        /// <summary>
        /// Sets all main components of the instance nodes.
        /// </summary>
        private void InitInstanceNodes()
        {
            document.Traverse(node =>
            {
                var instanceNode = (InstanceNode) node;
                if (string.IsNullOrEmpty(instanceNode.componentId))
                    throw new ArgumentException($"Instance node's {instanceNode.componentId} does not exist.");
                
               //// Find component definition.
               //if (!components.TryGetValue(instanceNode.componentId, out var component))
               //    throw new ArgumentException($"Component definition could not be found: {instanceNode.componentId}");

                // Find component node in the hierarchy.
                var componentNode = document.Find(instanceNode.componentId, NodeType.Component);
                if (componentNode == null)
                    throw new ArgumentException($"Component node could not be found: {instanceNode.componentId}");

                instanceNode.mainComponent = (ComponentNode) componentNode;
                return true;
            }, NodeType.Instance);
        }
        
        /// <summary>
        /// Sets all component sets of the components if exists.
        /// </summary>
        private void InitComponentNodes()
        {
            document.Traverse(node =>
            {
                var componentNode = (ComponentNode) node;
                
                if (!components.TryGetValue(componentNode.id, out var component))
                    throw new ArgumentException($"Component definition could not be found: {componentNode.id}");

                if (!string.IsNullOrEmpty(component.componentSetId))
                {
                    //if (!componentSets.TryGetValue(component.componentSetId, out var componentSet))
                    //    throw new ArgumentException(
                    //        $"Component set definition could not be found: {component.componentSetId}");

                    var componentSetNode = (ComponentSetNode) document.Find(component.componentSetId, NodeType.ComponentSet);
                    if (componentSetNode == null)
                        throw new ArgumentException($"Component set node could not be found: {component.componentSetId}");

                    componentNode.componentSet = componentSetNode;
                }
                return true;
            }, NodeType.Component);
        }
        
        private static void BuildHierarchy(Node node)
        {
            if (node.hasChildren)
            {
                foreach (var child in node.GetChildren())
                {
                    child.parent = node;
                    
                    BuildHierarchy(child);
                }
            }
        }
        
        public static FigmaFileContent FromString(string json) => 
            JsonConvert.DeserializeObject<FigmaFileContent>(json, JsonSerializerHelper.Settings);

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
                    return JsonConvert.SerializeObject(this, Formatting.Indented, JsonSerializerHelper.Settings);
                case "N":
                    return JsonConvert.SerializeObject(this, Formatting.None, JsonSerializerHelper.Settings);
                default:
                    throw new FormatException($"The {format} format string is not supported.");
            }
        }
    }
}