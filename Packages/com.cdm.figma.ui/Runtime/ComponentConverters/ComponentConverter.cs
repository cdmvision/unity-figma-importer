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
            // Debug.Log($"Instance name: {instanceNode.name}");
            
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
                // Debug.Log($"Property variants: {string.Join(",", propertyVariants)}");

                if (TryGetSelector(propertyVariants, out var selector))
                {
                    var frameNodeConverter = new FrameNodeConverter();
                    var nodeVariant = frameNodeConverter.Convert(parentObject, componentVariant, args);

                    try
                    {
                        MergeComponentVariant(instanceObject, nodeVariant, selector);
                        ApplyStyleSelectorsRecurse(instanceObject);   
                    }
                    finally
                    {
                        // We don't need variant object.
                        if (nodeVariant != null)
                        {
                            ObjectUtils.Destroy(nodeVariant.gameObject);
                        }
                    }
                }
            }
        }

        private void MergeComponentVariant(NodeObject node, NodeObject variant, Selector selector)
        {
            if (node.transform.childCount != variant.transform.childCount)
                throw new ArgumentException("Component variant has invalid number of children!");

            var styles = variant.styles;
            foreach (var style in styles)
            {
                style.selector = selector.ToString();
            }

            node.styles.AddRange(styles);

            for (var i = 0; i < node.transform.childCount; i++)
            {
                var nextNode = node.transform.GetChild(i).GetComponent<NodeObject>();
                var nextVariant = variant.transform.GetChild(i).GetComponent<NodeObject>();

                MergeComponentVariant(nextNode, nextVariant, selector);
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