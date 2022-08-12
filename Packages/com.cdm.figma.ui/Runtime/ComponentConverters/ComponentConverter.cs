using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class ComponentConverter : NodeConverter<InstanceNode>
    {
        private readonly Dictionary<string, List<string>> _variants = new Dictionary<string, List<string>>();
        protected IReadOnlyDictionary<string, List<string>> variants => _variants;
        
        protected ComponentConverter()
        {
        }

        protected abstract bool CanConvertType(string typeID);
        protected abstract bool TryGetSelector(string[] variant, out string selector);

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

            // Just take ComponentPropertyType.InstanceSwap into account. Others will be handled differently.
            var componentPropertyAssignments = instanceNode.componentProperties?.assignments?
                .Where(x => x.Value.type == ComponentPropertyType.InstanceSwap);
            
            if (componentPropertyAssignments != null)
            {
                foreach (var assignment in componentPropertyAssignments)
                {
                    var assignmentInstanceSwap = (ComponentPropertyAssignmentInstanceSwap) assignment.Value;
                    var componentId = assignmentInstanceSwap.value;

                    if (args.fileContent.componentNodes.TryGetValue(componentId, out var component))
                    {
                        args.componentPropertyAssignments.Add(assignment.Key, component);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Instance swap property assignment with key '${assignment.Key}' component could not be found: ${componentId}");
                    }
                }    
            }
            

            // Initialize property variants dictionary.
            foreach (var componentVariant in componentSetVariants)
            {
                var propertyVariants =
                    componentVariant.name.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                        .ToArray();

                foreach (var propertyVariant in propertyVariants)
                {
                    var tokens = propertyVariant.Split("=");
                    var key = tokens[0];
                    var value = tokens[1];
                    
                    if (!_variants.ContainsKey(key))
                    {
                        _variants.Add(key, new List<string>());
                    }
                    _variants[key].Add(value);
                }
            }

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
            
            args.componentPropertyAssignments.Clear();
        }

        private void MergeComponentVariant(NodeObject node, NodeObject variant, string selector)
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
            // Remove default styles.
            nodeObject.styles.RemoveAll(s => string.IsNullOrEmpty(s.selector));
            nodeObject.ApplyStylesSelectors();

            foreach (Transform child in nodeObject.transform)
            {
                ApplyStyleSelectorsRecurse(child.GetComponent<NodeObject>());
            }
        }

        protected bool TryGetSelector(string[] variant, ComponentProperty property, ref string selector)
        {
            if (IsSameVariant(variant, property))
            {
                if (string.IsNullOrEmpty(selector))
                {
                    selector = property.value;    
                }
                else
                {
                    selector += $":{property.value}";
                }
                
                return true;
            }

            return false;
        }

        protected bool IsSameVariant(string[] variant, params ComponentProperty[] query)
        {
            foreach (var q in query)
            {
                if (!variant.Contains(q.ToString()))
                    return false;
            }

            return true;
        }
    }

    public abstract class ComponentConverter<TComponent, TComponentVariantFilter> : ComponentConverter
        where TComponent : Selectable
        where TComponentVariantFilter : ComponentVariantFilter
    {
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var selectable = nodeObject.gameObject.AddComponent<TComponent>();
                selectable.transition = Selectable.Transition.None;
                
                var variantFilter = nodeObject.gameObject.AddComponent<TComponentVariantFilter>();
                variantFilter.Initialize();
            }

            return nodeObject;
        }
    }
}