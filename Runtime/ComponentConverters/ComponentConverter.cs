﻿using System;
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
            InitVariantsDictionary(instanceNode);

            using (args.ImportingComponentSet())
            {
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
                        }
                        catch (Exception e)
                        {
                            args.importer.LogError(e, instanceObject);
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
            }
            
            ApplyStyleSelectorsRecurse(instanceObject);
            DisableTextOverrideIfSameForAll(instanceObject);
            ClearComponentPropertyAssignments(args);
        }

        private void InitVariantsDictionary(InstanceNode instanceNode)
        {
            _variants.Clear();

            foreach (var componentVariant in instanceNode.mainComponent.componentSet.variants)
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
        }

        private static void SetComponentPropertyAssignments(InstanceNode instanceNode, NodeConvertArgs args)
        {
            if (instanceNode.componentProperties != null)
            {
                foreach (var componentProperty in instanceNode.componentProperties)
                {
                    if (componentProperty.Value.type == ComponentPropertyType.InstanceSwap)
                    {
                        var instanceSwapProperty = (ComponentPropertyInstanceSwap)componentProperty.Value;
                        var componentId = instanceSwapProperty.value;

                        if (args.file.componentNodes.TryGetValue(componentId, out var component))
                        {
                            args.componentPropertyAssignments.Add(componentProperty.Key, component);
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Instance swap property assignment with key '${componentProperty.Key}' component could not be found: ${componentId}");
                        }
                    }
                    else if (componentProperty.Value.type == ComponentPropertyType.Text)
                    {
                        var textProperty = (ComponentPropertyText)componentProperty.Value;
                        var characters = textProperty.value;

                        args.textPropertyAssignments.Add(componentProperty.Key, characters);
                    }
                }
            }
        }

        private static void ClearComponentPropertyAssignments(NodeConvertArgs args)
        {
            args.componentPropertyAssignments.Clear();
            args.textPropertyAssignments.Clear();
        }

        private static void MergeComponentVariant(FigmaNode node, FigmaNode variant, string selector)
        {
            var nodeChildren = node.GetChildren();
            var variantChildren = variant.GetChildren();

            if (nodeChildren.Count != variantChildren.Count)
                throw new ArgumentException("Component variant has invalid number of children!");

            var styles = variant.styles;
            foreach (var style in styles)
            {
                style.selector = selector;
            }

            node.styles.AddRange(styles);

            for (var i = 0; i < nodeChildren.Count; i++)
            {
                var nextNode = nodeChildren[i];
                var nextVariant = variantChildren[i];

                MergeComponentVariant(nextNode, nextVariant, selector);
            }
        }

        private static void ApplyStyleSelectorsRecurse(FigmaNode figmaNode)
        {
            // Remove default styles.
            figmaNode.styles.RemoveAll(s => string.IsNullOrEmpty(s.selector));

            // Apply default styles.
            foreach (var style in figmaNode.styles)
            {
                style.SetStyleAsSelector(figmaNode.gameObject, new StyleArgs("", true));
            }
            
            // Do the same for the node's children.
            var children = figmaNode.GetChildren();
            foreach (var child in children)
            {
                ApplyStyleSelectorsRecurse(child);
            }
        }

        private static void DisableTextOverrideIfSameForAll(FigmaNode figmaNode)
        {
            var textStyles = figmaNode.styles.OfType<TextStyle>().ToArray();
            if (textStyles.Length > 0)
            {
                var isTextSame = true;
                for (var i = 0; i < textStyles.Length; i++)
                {
                    for (var j = i + 1; j < textStyles.Length; j++)
                    {
                        if (textStyles[i].text.value != textStyles[j].text.value)
                        {
                            //Debug.Log($"'{textStyles[i].text.value}' != '{textStyles[j].text.value}'", figmaNode);
                            isTextSame = false;
                            break;
                        }
                    }
                }

                if (isTextSame)
                {
                    figmaNode.DisableTextStyleTextOverride();
                }
            }
            
            foreach (var child in figmaNode)
            {
                DisableTextOverrideIfSameForAll(child);
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