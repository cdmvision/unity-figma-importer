using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class SliderComponentConverter<TSlider, TComponentVariantFilter>
        : SelectableComponentConverter<TSlider, TComponentVariantFilter>
        where TSlider : Slider
        where TComponentVariantFilter : SelectableComponentVariantFilter
    {
        protected const string HandleKey = BindingPrefix + "Handle";
        protected const string FillKey = BindingPrefix + "Fill";

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var figmaNode = base.Convert(parentObject, instanceNode, args);

            if (figmaNode != null)
            {
                if (!figmaNode.TryFindNode<RectTransform>(args, FillKey, out var fill))
                {
                    return figmaNode;
                }

                // Disable transform style to prevent slider layout change overrides.
                RemoveStyleSetter<TransformStyleSetter>(fill.gameObject);
                RemoveStyleSetter<TransformStyleSetter>(fill.transform.parent.gameObject);

                var slider = figmaNode.GetComponent<Slider>();
                slider.fillRect = fill;
                slider.fillRect.offsetMin = Vector2.zero;
                slider.fillRect.offsetMax = Vector2.zero;

                // Handle is not mandatory.
                if (figmaNode.TryFindOptionalNode<RectTransform>(HandleKey, out var handle))
                {
                    slider.handleRect = handle;

                    // Handle pivot should be the center of the handle rect.
                    slider.handleRect.pivot = Vector2.one * 0.5f;
                    slider.handleRect.anchoredPosition = Vector2.zero;

                    RemoveStyleSetter<TransformStyleSetter>(handle.gameObject);
                    RemoveStyleSetter<TransformStyleSetter>(handle.transform.parent.gameObject);
                }

                ReinitializeVariantFilter(figmaNode);

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

            return figmaNode;
        }
    }
    
    public class SliderComponentConverter : SliderComponentConverter<Slider, SelectableComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Slider";
        }
    }
}