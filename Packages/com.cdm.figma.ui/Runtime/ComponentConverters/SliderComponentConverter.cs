using System;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class SliderComponentConverter : SelectableComponentConverter<Slider, SelectableComponentVariantFilter>
    {
        private const string HandleKey = "@Handle";
        private const string FillKey = "@Fill";

        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Slider";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);

            if (nodeObject != null)
            {
                var slider = nodeObject.GetComponent<Slider>();

                var handle = nodeObject.Find(x => x.bindingKey == HandleKey);
                if (handle == null)
                    throw new ArgumentException(
                        $"Slider handle node could not be found. Did you set '{HandleKey}' as binding key?");

                var fill = nodeObject.Find(x => x.bindingKey == FillKey);
                if (fill == null)
                    throw new ArgumentException(
                        $"Slider fill node could not be found. Did you set '{FillKey}' as binding key?");

                slider.fillRect = fill.rectTransform;
                slider.handleRect = handle.rectTransform;
                
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