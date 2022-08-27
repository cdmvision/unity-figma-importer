using System;
using Cdm.Figma.UI.Styles;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class SliderComponentConverter : SelectableComponentConverter<Slider, SelectableComponentVariantFilter>
    {
        private const string HandleKey = BindingPrefix + "Handle";
        private const string FillKey = BindingPrefix + "Fill";

        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Slider";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var slider = nodeObject.GetComponent<Slider>();

                var fill = nodeObject.Find(x => x.bindingKey == FillKey);
                if (fill == null)
                    throw new ArgumentException(
                        $"Slider fill node could not be found. Did you set '{FillKey}' as binding key?");
                
                slider.fillRect = fill.rectTransform;
                
                // Disable transform style to prevent slider layout change overrides.
                RemoveStyleSetter<TransformStyleSetter>(fill.gameObject);
                RemoveStyleSetter<TransformStyleSetter>(fill.transform.parent.gameObject);
                
                // Handle is not mandatory.
                var handle = nodeObject.Find(x => x.bindingKey == HandleKey);
                if (handle != null)
                {
                    slider.handleRect = handle.rectTransform;
                    
                    // Handle pivot should be the center of the handle rect.
                    slider.handleRect.pivot = Vector2.one * 0.5f;
                    slider.handleRect.anchoredPosition = Vector2.zero;
                    
                    RemoveStyleSetter<TransformStyleSetter>(handle.gameObject);
                    RemoveStyleSetter<TransformStyleSetter>(handle.transform.parent.gameObject);    
                }

                ReinitializeVariantFilter(nodeObject);

                if (instanceNode.mainComponent.componentSet != null &&
                    instanceNode.mainComponent.componentSet.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<SliderComponentData>();
                    if (componentData != null)
                    {
                        slider.direction = componentData.direction;
                    }
                }
            }

            return nodeObject;
        }
    }
}