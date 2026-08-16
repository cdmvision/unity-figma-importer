using System;
using Cdm.Figma.UI.Styles;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public abstract class ScrollbarComponentConverter<TScrollbar, TSelectableComponentVariantFilter>
        : SelectableComponentConverter<TScrollbar, TSelectableComponentVariantFilter>
        where TScrollbar : Scrollbar
        where TSelectableComponentVariantFilter : SelectableComponentVariantFilter
    {
        private const string HandleKey = BindingPrefix + "Handle";

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var scrollbar = nodeObject.GetComponent<Scrollbar>();

                var handle = nodeObject.Find(x => x.bindingKey == HandleKey);
                if (handle == null)
                    throw new ArgumentException(
                        $"Slider handle node could not be found. Did you set '{HandleKey}' as binding key?");

                scrollbar.handleRect = handle.rectTransform;

                // Disable transform style to prevent slider layout change overrides.
                RemoveStyleSetter<TransformStyleSetter>(handle.gameObject);
                RemoveStyleSetter<TransformStyleSetter>(handle.transform.parent.gameObject);

                ReinitializeVariantFilter(nodeObject);

                if (instanceNode.mainComponent.componentSet != null &&
                    instanceNode.mainComponent.componentSet.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<ScrollbarComponentData>();
                    if (componentData != null)
                    {
                        scrollbar.direction = componentData.direction;
                    }
                }
            }

            return nodeObject;
        }
    }

    public class ScrollbarComponentConverter : ScrollbarComponentConverter<Scrollbar, SelectableComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Scrollbar";
        }
    }
}