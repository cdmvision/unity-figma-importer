using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public static class ComponentState
    {
        public const string Default = "Default";
        public const string Hover = "Hover";
        public const string Press = "Press";
        public const string Disabled = "Disabled";
        public const string Selected = "Selected";
    }
    
    public abstract class ComponentConverter : NodeConverter<InstanceNode>
    {
        public string typeId { get; set; }
        
        private List<ComponentProperty> _properties = new List<ComponentProperty>();

        public List<ComponentProperty> properties
        {
            get => _properties;
            protected set => _properties = value ?? new List<ComponentProperty>();
        }

        public ComponentConverter()
        {
        }
        
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (!base.CanConvert(node, args))
                return false;
            
            var instanceNode = (InstanceNode) node;
            if (instanceNode.mainComponent == null)
                return false;

            var componentType = "";
            if (instanceNode.mainComponent.componentSet != null)
            {
                componentType = instanceNode.mainComponent.componentSet.GetComponentType();
            }
            else
            {
                componentType = instanceNode.mainComponent.GetComponentType();
            }

            if (string.IsNullOrEmpty(componentType) || componentType != typeId)
                return false;
            
            // Everything is OK.
            return true;
        }
        
        protected bool SameVariant(string[] variant, params string[] query)
        {
            if (variant.Length.Equals(query.Length))
            {
                Array.Sort(variant);
                Array.Sort(query);
                
                for (var i = 0; i < variant.Length; i++)
                {
                    if (variant[i] != query[i])
                    {
                        return false;
                    }
                }

                return true;

            }

            return false;
        }
    }

    public abstract class ComponentConverter<TComponent> : ComponentConverter
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;
            var mainComponent = instanceNode.mainComponent;

            var rootElement = NodeElement.New<TComponent>(node, args);
            var rootStyleClass = GetNodeStyleClass(node);
            rootElement.value.Add(new XAttribute("class", rootStyleClass));
            
            if (mainComponent.componentSet != null)
            {
                var componentSet = mainComponent.componentSet;
                var componentSetVariants = componentSet.variants;

                // These are the variants as components.
                var componentElements = new List<KeyValuePair<string, NodeElement>>();

                foreach (var componentVariant in componentSetVariants)
                {
                    var propertyVariants = 
                        componentVariant.name.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

                    var componentElement = NodeElement.New<VisualElement>(node, args);
                    
                    if (TryGetSelector(propertyVariants, out var selector))
                    {
                        var baseSelector = $".{rootStyleClass}{selector}";
                        var styleDefinition = new StyleDefinition(baseSelector);
                        rootElement.styles.Add(styleDefinition);
                        
                        // Debug.Log($"Selector: {selector} Variant: {string.Join(", ", propertyVariants)}");
                        
                        if (componentVariant.children != null)
                        {
                            foreach (var child in componentVariant.children)
                            {
                                if (args.importer.TryConvertNode(child, args, out var childElement))
                                {
                                    componentElement.AddChild(childElement);       
                                }
                            }
                        }

                        componentElements.Add(new KeyValuePair<string, NodeElement>(baseSelector, componentElement));
                    }
                    else
                    {
                        Debug.LogWarning($"Variant selector could not be get: {string.Join(", ", propertyVariants)}");
                    }
                }

                // Merge all variant hierarchy to first one.
                MoveInlineStyleToDefinitionRecurse(componentElements[0].Key, componentElements[0].Value);
                
                for (var i = 1; i < componentElements.Count; i++)
                {
                    MergeNodeElementRecurse(
                        componentElements[i].Key, componentElements[i].Value, componentElements[0].Value);
                }

                // Ignore Component node. It will be the Instance node.
                var children = componentElements[0].Value.children;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        rootElement.AddChild(child);       
                    }
                }
            }
            else
            {
                 // TODO: convert single component node!
            }
            
            return rootElement;
        }
        
        protected static string GetNodeStyleClass(Node node)
        {
            return $"n{node.id.Replace(":", "_").Replace(";", "__")}";
        }

        private static void MoveInlineStyleToDefinitionRecurse(string selector, NodeElement element)
        {
            var styleClass = GetNodeStyleClass(element.node);
            element.styles.Add(new StyleDefinition($"{selector} > .{styleClass}", element.inlineStyle));
            element.value.Add(new XAttribute("class", styleClass));
            element.ClearStyle();

            if (element.children != null)
            {
                foreach (var child in element.children)
                {
                    MoveInlineStyleToDefinitionRecurse(selector, child);
                }
            }
        }

        private static void MergeNodeElementRecurse(string baseSelector, NodeElement from, NodeElement to)
        {
            if (from.node.type != to.node.type)
                throw new Exception($"Both node types must be the same: from: {from.node.type} to: {to.node.type}");
            
            to.styles.Add(
                new StyleDefinition($"{baseSelector} > .{GetNodeStyleClass(to.node)}", from.inlineStyle));

            if (to.children != null)
            {
                if (from.children == null)
                    throw new Exception("from.children is null.");
                
                if (to.children.Count != from.children.Count)
                    throw new Exception("Both nodes must have same children count.");

                for (var i = 0; i < to.children.Count; i++)
                {
                    MergeNodeElementRecurse(baseSelector, from.children[i], to.children[i]);
                }
            } 
            else if (from.children != null)     // If to.children is null, from.children must be null also.
            {
                throw new Exception("from.children is not null");
            }
        }
        
        protected abstract bool TryGetSelector(string[] variant, out string selector);
    }
}