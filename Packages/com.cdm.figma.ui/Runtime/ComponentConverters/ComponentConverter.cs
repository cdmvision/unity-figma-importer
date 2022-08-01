using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
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
    }

    public abstract class ComponentConverter : NodeConverter<InstanceNode>
    {
        private List<ComponentProperty> _properties = new List<ComponentProperty>();

        public List<ComponentProperty> properties
        {
            get => _properties;
            protected set => _properties = value ?? new List<ComponentProperty>();
        }

        protected ComponentConverter()
        {
        }

        protected abstract bool CanConvertType(string typeID);
        protected abstract bool TryGetSelector(string[] variant, out Selector selector);

        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (!base.CanConvert(node, args))
                return false;

            var instanceNode = (InstanceNode)node;
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

            return CanConvertType(componentType);
        }

        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            Debug.Log($"Instance name: {instanceNode.name}");
            if (instanceNode.mainComponent.componentSet != null)
            {
                Debug.Assert(instanceNode.mainComponent != null);

                var nodeObject = new FrameNodeConverter().Convert(parentObject, instanceNode, args);
                ConvertComponentSet(nodeObject, parentObject, instanceNode, args);
                return nodeObject;
            }

            var instanceNodeConverter = new InstanceNodeConverter();
            return instanceNodeConverter.Convert(parentObject, instanceNode, args);
        }

        private void ConvertComponentSet(NodeObject instanceObject, NodeObject parentObject, InstanceNode instanceNode,
            NodeConvertArgs args)
        {
            var mainComponent = instanceNode.mainComponent;
            var componentSet = mainComponent.componentSet;
            var componentSetVariants = componentSet.variants;

            foreach (var componentVariant in componentSetVariants)
            {
                // Array of State=Hover, Checked=On etc.
                var propertyVariants =
                    componentVariant.name.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                        .ToArray();
                //Debug.Log($"Property variants: {string.Join(",", propertyVariants)}");

                if (TryGetSelector(propertyVariants, out var selector))
                {
                    ConvertComponentSetRecurse(parentObject, instanceObject, componentVariant, args, selector);
                    ApplyStyleSelectorsRecurse(instanceObject);
                    
                    /*if (componentVariant.hasChildren)
                    {
                        if (componentVariant.children.Length != instanceObject.transform.childCount)
                            throw new ArgumentException("Component variant ");
                            
                        for (var i = 0; i < componentVariant.children.Length; i++)
                        {
                            var child = componentVariant.children[i];
                            
                            if (args.importer.TryConvertNode(parentObject, child, args, out var variantNode))
                            {
                                var styles = variantNode.styles;

                                ObjectUtils.Destroy(variantNode.gameObject);

                                foreach (var style in styles)
                                {
                                    style.selector = selector.ToString();
                                }

                                var childNode = instanceObject.transform.GetChild(i).GetComponent<NodeObject>();
                                childNode.styles.AddRange(styles);
                            }
                        }

                        for (var i = 0; i < instanceObject.transform.childCount; i++)
                        {
                            var childNode = instanceObject.transform.GetChild(i).GetComponent<NodeObject>();
                            childNode.ApplyStylesSelectors();
                        }
                    }*/
                }
            }
        }

        private void ConvertComponentSetRecurse(NodeObject parentObject, NodeObject nodeObject, Node nodeVariant, 
            NodeConvertArgs args, Selector selector)
        {
            if (args.importer.TryConvertNode(parentObject, nodeVariant, args, out var variantNode))
            {
                var styles = variantNode.styles;

                ObjectUtils.Destroy(variantNode.gameObject);

                foreach (var style in styles)
                {
                    style.selector = selector.ToString();
                }
                
                Debug.Log("add " + styles.Count);
                nodeObject.styles.AddRange(styles);
                
                if (nodeObject.transform.childCount > 0 && !nodeVariant.hasChildren)
                    throw new ArgumentException("Component variant has invalid number of children!");
                
                if (nodeVariant.hasChildren)
                {
                    var variantChildren = nodeVariant.GetChildren();
                    if (variantChildren.Length != nodeObject.transform.childCount)
                        throw new ArgumentException("Component variant has invalid number of children!");

                    for (var i = 0; i < variantChildren.Length; i++)
                    {
                        var nextNodeVariant = variantChildren[i];
                        var nextNodeObject = nodeObject.transform.GetChild(i).GetComponent<NodeObject>();

                        ConvertComponentSetRecurse(nodeObject, nextNodeObject, nextNodeVariant, args, selector);
                    }
                }
            }
        }

        private void ApplyStyleSelectorsRecurse(NodeObject nodeObject)
        {
            nodeObject.ApplyStylesSelectors();

            foreach (Transform child in nodeObject.transform)
            {
                ApplyStyleSelectorsRecurse(child.GetComponent<NodeObject>());
            }
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
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var selectable = nodeObject.gameObject.AddComponent<TComponent>();
                selectable.transition = Selectable.Transition.None;

                var selectableGroup = nodeObject.gameObject.AddComponent<SelectableGroup>();
                selectableGroup.InitializeComponents();
            }

            return nodeObject;
        }
    }
}