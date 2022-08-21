using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ScrollViewComponentConverter : ComponentConverter
    {
        private const string ContentKey = BindingPrefix + "Content";
        private const string ViewportKey = BindingPrefix + "Viewport";
        private const string HorizontalScrollbarKey = BindingPrefix + "ScrollbarHorizontal";
        private const string VerticalScrollbarKey = BindingPrefix + "ScrollbarVertical";

        protected override bool CanConvertType(string typeID)
        {
            return typeID == "ScrollView";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            if (nodeObject != null)
            {
                var scrollRect = nodeObject.gameObject.AddComponent<ScrollRect>();
                var content = nodeObject.Find(x => x.bindingKey == ContentKey);
                if (content == null)
                    throw new ArgumentException(
                        $"ScrollView content node could not be found. Did you set '{ContentKey}' as binding key?");

                var viewport = nodeObject.Find(x => x.bindingKey == ViewportKey);
                if (viewport == null)
                    throw new ArgumentException(
                        $"ScrollView viewport node could not be found. Did you set '{ViewportKey}' as binding key?");

                var horizontalScrollbar = nodeObject.Find(x => x.bindingKey == HorizontalScrollbarKey);
                var verticalScrollbar = nodeObject.Find(x => x.bindingKey == VerticalScrollbarKey);

                if (horizontalScrollbar != null)
                {
                    scrollRect.horizontalScrollbar = horizontalScrollbar.rectTransform.GetComponent<Scrollbar>();
                    scrollRect.horizontalScrollbarSpacing = -3; // TODO: auto calculate
                }

                if (verticalScrollbar != null)
                {
                    scrollRect.verticalScrollbar = verticalScrollbar.rectTransform.GetComponent<Scrollbar>();
                    scrollRect.verticalScrollbarSpacing = -3;   // TODO: auto calculate 
                }
                
                if (instanceNode.mainComponent.componentSet != null &&
                    instanceNode.mainComponent.componentSet.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<ScrollViewComponentData>();
                    if (componentData != null)
                    {
                        scrollRect.horizontalScrollbarVisibility = componentData.horizontalVisibility;
                        scrollRect.verticalScrollbarVisibility = componentData.verticalVisibility;
                    }
                }

                scrollRect.content = content.rectTransform;
                scrollRect.viewport = viewport.rectTransform;
                
                var contentSizeFitter = scrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            return nodeObject;
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            selector = "";
            return false;
        }
    }
}