using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Styles;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
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
        
        protected ComponentConverter()
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

            return !string.IsNullOrEmpty(componentType) && componentType == typeId;
        }
        
        protected bool IsSameVariant(string[] variant, params string[] query)
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

    public abstract class ComponentConverter<TComponent> : ComponentConverter where TComponent : Selectable
    {
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            Debug.Log($"Instance name: {instanceNode.name}");
            if (instanceNode.mainComponent.componentSet != null)
            {
                Debug.Assert(instanceNode.mainComponent != null);
            
                var nodeObject = new FrameNodeConverter().Convert(parentObject, instanceNode, args);
                var selectable = nodeObject.gameObject.AddComponent<TComponent>();
                selectable.transition = Selectable.Transition.None;

                nodeObject.gameObject.AddComponent<SelectableGroup>();
                
                ConvertComponentSet(nodeObject, parentObject, instanceNode, args);
                return nodeObject;
            }

            var instanceNodeConverter = new InstanceNodeConverter();
            return instanceNodeConverter.Convert(parentObject, instanceNode, args);
        }

        private void ConvertComponentSet(NodeObject instanceObject, NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var mainComponent = instanceNode.mainComponent;
            var componentSet = mainComponent.componentSet;
            var componentSetVariants = componentSet.variants;

            foreach (var componentVariant in componentSetVariants)
            {
                // Array of State=Hover, Checked=On etc.
                var propertyVariants = 
                    componentVariant.name.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                Debug.Log($"Property variants: {string.Join("," ,propertyVariants)}");

                if (TryGetSelector(propertyVariants, out var selector))
                {
                    if (componentVariant.hasChildren)
                    {
                        foreach (var child in componentVariant.children)
                        {
                            if (args.importer.TryConvertNode(parentObject, child, args, out var nodeObject))
                            {
                                nodeObject.name = $"{nodeObject.name}:{selector}";
                                
                                nodeObject.rectTransform.SetParent(instanceObject.rectTransform, false);
                            }
                        }
                    }
                }
            }
            
        }

        private static void SetStyleRecurse(Node node)
        {
            
        }
        
        
        protected abstract bool TryGetSelector(string[] variant, out Selector selector);
    }
}