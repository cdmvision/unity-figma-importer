using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ScrollbarComponentConverter : SelectableComponentConverter<Scrollbar, SelectableComponentVariantFilter>
    {
        private const string HandleKey = "@Handle";
        
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Scrollbar";
        }
        
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            
            if (nodeObject != null)
            {
                var scrollbar = nodeObject.GetComponent<Scrollbar>();

                var handle = nodeObject.Find(x => x.bindingKey == HandleKey);
                if (handle == null)
                    throw new ArgumentException($"Slider handle node could not be found. Did you set '{HandleKey}' as binding key?");

                scrollbar.handleRect = handle.rectTransform;

                if (instanceNode.mainComponent.componentSet != null &&
                    instanceNode.mainComponent.componentSet.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<ScrollbarComponentData>();
                    if (componentData != null)
                    {
                        scrollbar.direction = componentData.direction;
                    }
                }
                
                // TODO: Bug ?
                var handleParent = (RectTransform) handle.rectTransform.parent;
                handleParent.offsetMin = new Vector2(handleParent.offsetMin.x, handleParent.offsetMin.y + 1);
            }

            return nodeObject;
        }
    }
}