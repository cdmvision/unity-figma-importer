using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

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
    public class FigmaFile
    {
        [DataMember]
        public string fileId { get; set; }

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

        [DataMember(Name = "thumbnail", Order = int.MaxValue)]
        public string thumbnail { get; set; }

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

        [DataMember(Name = "fileDependencies")]
        public FigmaFileDependency[] fileDependencies { get; set; }

        private readonly Dictionary<string, ComponentNode> _componentNodes = new Dictionary<string, ComponentNode>();

        /// <summary>
        /// All <see cref="ComponentNode"/>s in the document.
        /// </summary>
        /// <remarks>
        /// It is available after <see cref="BuildHierarchy"/> method is called.
        /// </remarks>
        public IReadOnlyDictionary<string, ComponentNode> componentNodes => _componentNodes;

        private readonly Dictionary<string, ComponentSetNode> _componentSetNodes =
            new Dictionary<string, ComponentSetNode>();

        /// <summary>
        /// All <see cref="ComponentSetNode"/>s in the document. 
        /// </summary>
        /// <remarks>
        /// It is available after <see cref="BuildHierarchy"/> method is called.
        /// </remarks>
        public IReadOnlyDictionary<string, ComponentSetNode> componentSetNodes => _componentSetNodes;

        public void BuildHierarchy()
        {
            _componentNodes.Clear();
            _componentSetNodes.Clear();

            // Handle main file.
            BuildHierarchyRecurse(document);
            FixRelativePositionGroupNodeChildren(document);

            // Handle file dependencies.
            BuildHierarchyForFileDependencies();
            FixRelativePositionGroupNodeChildrenForFileDependencies();
        }

        private void BuildHierarchyForFileDependencies()
        {
            if (fileDependencies != null)
            {
                foreach (var fileReference in fileDependencies)
                {
                    foreach (var node in fileReference.componentNodes.Values)
                    {
                        BuildHierarchyRecurse(node);
                    }

                    foreach (var node in fileReference.componentSetNodes.Values)
                    {
                        BuildHierarchyRecurse(node);
                    }
                }
            }
        }

        private void FixRelativePositionGroupNodeChildrenForFileDependencies()
        {
            if (fileDependencies != null)
            {
                foreach (var fileReference in fileDependencies)
                {
                    foreach (var node in fileReference.componentNodes.Values)
                    {
                        FixRelativePositionGroupNodeChildren(node);
                    }

                    foreach (var node in fileReference.componentSetNodes.Values)
                    {
                        FixRelativePositionGroupNodeChildren(node);
                    }
                }
            }
        }

        private void BuildHierarchyRecurse(Node node)
        {
            if (node is ComponentNode componentNode)
            {
                _componentNodes.TryAdd(componentNode.id, componentNode);
            }
            else if (node is ComponentSetNode componentSetNode)
            {
                _componentSetNodes.TryAdd(componentSetNode.id, componentSetNode);
            }

            if (node.hasChildren)
            {
                foreach (var child in node.GetChildren())
                {
                    child.parent = node;

                    BuildHierarchyRecurse(child);
                }
            }
        }

        public InstanceNodeInitResult InitInstanceNode(InstanceNode instanceNode)
        {
            if (string.IsNullOrEmpty(instanceNode.componentId))
            {
                return InstanceNodeInitResult.MissingComponentID;
            }

            // Find component node in the main file hierarchy.
            if (_componentNodes.TryGetValue(instanceNode.componentId, out var componentNode))
            {
                instanceNode.mainComponent = componentNode;
                return InitComponentNode(componentNode, components, _componentSetNodes);
            }

            // Try to find component node in file dependencies.
            if (fileDependencies != null)
            {
                if (components.TryGetValue(instanceNode.componentId, out var component))
                {
                    foreach (var fileReference in fileDependencies)
                    {
                        foreach (var referenceComponent in fileReference.components)
                        {
                            var componentKey = referenceComponent.Value.key;
                            if (componentKey == component.key)
                            {
                                var componentNodeId = referenceComponent.Key;

                                if (fileReference.componentNodes.TryGetValue(componentNodeId, out var componentNode2))
                                {
                                    instanceNode.mainComponent = componentNode2;
                                    return InitComponentNode(componentNode2, fileReference.components,
                                        fileReference.componentSetNodes);
                                }
                            }
                        }
                    }
                }
            }

            return InstanceNodeInitResult.MissingComponent;
        }

        private static InstanceNodeInitResult InitComponentNode(ComponentNode componentNode,
            IReadOnlyDictionary<string, Component> components,
            IReadOnlyDictionary<string, ComponentSetNode> componentSetNodes)
        {
            if (!components.TryGetValue(componentNode.id, out var component))
            {
                return InstanceNodeInitResult.MissingComponentDefinition;
            }

            if (!string.IsNullOrEmpty(component.componentSetId))
            {
                if (!componentSetNodes.TryGetValue(component.componentSetId, out var componentSetNode))
                {
                    return InstanceNodeInitResult.MissingComponentSet;
                }

                componentNode.componentSet = componentSetNode;
            }

            return InstanceNodeInitResult.Success;
        }


        private static void FixRelativePositionGroupNodeChildren(Node node)
        {
            // Accumulate relative position for nodes that is child of a group node.
            // Frame
            //  - Group 1
            //      - Group 4
            //          - Rect 1
            //          - Rect 2
            //      - Group 5
            //          - Rect 3
            //  - Group 6
            //      - Rect 4
            //  - Frame 2
            //      - Group 7
            //          - Rect 5

            node.TraverseDfs(current =>
            {
                if (current.type != NodeType.Group)
                {
                    FixRelativePositionGroupNodeChild(current);
                }

                return true;
            });
        }

        private static void FixRelativePositionGroupNodeChild(Node node)
        {
            if (node is INodeTransform nodeTransform)
            {
                var position = nodeTransform.relativeTransform.position;

                for (var parent = node.parent; parent != null && parent.type == NodeType.Group; parent = parent.parent)
                {
                    var groupTransform = (INodeTransform)parent;
                    position += groupTransform.relativeTransform.position;
                }

                nodeTransform.relativeTransform.position = position;
            }
        }

        public static FigmaFile Parse(string json)
        {
            return JsonConvert.DeserializeObject<FigmaFile>(json, JsonSerializerHelper.Settings);
        }


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

    public enum InstanceNodeInitResult
    {
        Success,
        MissingComponentID,
        MissingComponent,
        MissingComponentDefinition,
        MissingComponentSet
    }
}