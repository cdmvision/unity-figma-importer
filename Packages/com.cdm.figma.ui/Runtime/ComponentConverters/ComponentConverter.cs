using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class ComponentConverter : NodeConverter<InstanceNode>
    {
        public const string BindingPrefix = "@";

        protected abstract bool CanConvertType(string typeId);

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

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var instanceNodeConverter = new InstanceNodeConverter();
            return instanceNodeConverter.Convert(parentObject, instanceNode, args);
        }
    }

    public abstract class ComponentConverterWithVariants : ComponentConverter
    {
        private readonly Dictionary<string, List<string>> _variants = new Dictionary<string, List<string>>();
        protected IReadOnlyDictionary<string, List<string>> variants => _variants;

        protected ComponentConverterWithVariants()
        {
        }

        protected abstract bool TryGetSelector(string[] variant, out string selector);

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            // Debug.Log($"Instance name: {instanceNode.name}");

            if (instanceNode.mainComponent.componentSet != null)
            {
                var nodeObject = new FrameNodeConverter().Convert(parentObject, instanceNode, args);
                ConvertComponentSet(nodeObject, parentObject, instanceNode, args);
                return nodeObject;
            }

            return base.Convert(parentObject, instanceNode, args);
        }

        private void ConvertComponentSet(FigmaNode instanceObject, FigmaNode parentObject, InstanceNode instanceNode,
            NodeConvertArgs args)
        {
            var mainComponent = instanceNode.mainComponent;
            var componentSet = mainComponent.componentSet;
            var componentSetVariants = componentSet.variants;

            SetComponentPropertyAssignments(instanceNode, args);
            
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
                    catch (Exception e)
                    {
                        args.importer.LogError(e, instanceObject.gameObject);
                        break;
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

            ClearComponentPropertyAssignments(args);
        }
        
        private static void SetComponentPropertyAssignments(InstanceNode instanceNode, NodeConvertArgs args)
        {
            var componentPropertyAssignments = instanceNode.componentProperties?.assignments;
            if (componentPropertyAssignments != null)
            {
                foreach (var assignment in componentPropertyAssignments)
                {
                    if (assignment.Value.type == ComponentPropertyType.InstanceSwap)
                    {
                        var assignmentInstanceSwap = (ComponentPropertyAssignmentInstanceSwap)assignment.Value;
                        var componentId = assignmentInstanceSwap.value;

                        if (args.file.componentNodes.TryGetValue(componentId, out var component))
                        {
                            args.componentPropertyAssignments.Add(assignment.Key, component);
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Instance swap property assignment with key '${assignment.Key}' component could not be found: ${componentId}");
                        }
                    }
                    else if (assignment.Value.type == ComponentPropertyType.Text)
                    {
                        var assignmentText = (ComponentPropertyAssignmentText)assignment.Value;
                        var characters = assignmentText.value;

                        args.textPropertyAssignments.Add(assignment.Key, characters);
                    }
                }
            }
        }

        private static void ClearComponentPropertyAssignments(NodeConvertArgs args)
        {
            args.componentPropertyAssignments.Clear();
            args.textPropertyAssignments.Clear();
        }

        private void MergeComponentVariant(FigmaNode node, FigmaNode variant, string selector)
        {
            if (node.transform.childCount != variant.transform.childCount)
                throw new ArgumentException("Component variant has invalid number of children!");

            var styles = variant.styles;
            foreach (var style in styles)
            {
                style.selector = selector;
            }

            node.styles.AddRange(styles);

            for (var i = 0; i < node.transform.childCount; i++)
            {
                var nextNode = node.transform.GetChild(i).GetComponent<FigmaNode>();
                var nextVariant = variant.transform.GetChild(i).GetComponent<FigmaNode>();

                MergeComponentVariant(nextNode, nextVariant, selector);
            }
        }

        private void ApplyStyleSelectorsRecurse(FigmaNode nodeObject)
        {
            // Remove default styles.
            nodeObject.styles.RemoveAll(s => string.IsNullOrEmpty(s.selector));
            nodeObject.ApplyStylesSelectors();

            foreach (Transform child in nodeObject.transform)
            {
                ApplyStyleSelectorsRecurse(child.GetComponent<FigmaNode>());
            }
        }

        protected static bool TryGetSelector(string[] variant, ComponentProperty property, ref string selector)
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

        private static bool IsSameVariant(string[] variant, params ComponentProperty[] query)
        {
            return query.All(q => variant.Contains(q.ToString()));
        }

        protected static void RemoveStyleSetter<T>(GameObject node) where T : StyleSetter
        {
            var styleSetter = node.GetComponent<T>();
            if (styleSetter != null)
            {
                ObjectUtils.Destroy(styleSetter);
            }
        }

        protected static void ReinitializeVariantFilter(FigmaNode mainNode)
        {
            var variantFilter = mainNode.gameObject.GetComponent<ComponentVariantFilter>();
            variantFilter.Initialize();
        }
    }

    public abstract class ComponentConverterWithVariants<TComponent, TComponentVariantFilter>
        : ComponentConverterWithVariants
        where TComponent : Selectable
        where TComponentVariantFilter : ComponentVariantFilter
    {
        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
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