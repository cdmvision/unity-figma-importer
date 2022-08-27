using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ScrollViewComponentConverter : ComponentConverterWithVariants
    {
        private const string ContentKey = BindingPrefix + "Content";
        private const string ViewportKey = BindingPrefix + "Viewport";
        private const string HorizontalScrollbarKey = BindingPrefix + "ScrollbarHorizontal";
        private const string VerticalScrollbarKey = BindingPrefix + "ScrollbarVertical";

        protected override bool CanConvertType(string typeId)
        {
            return typeId == "ScrollView";
        }

        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var figmaNode = base.Convert(parentObject, instanceNode, args);
            if (figmaNode != null)
            {
                if (!figmaNode.TryFindNode<RectTransform>(args, ContentKey, out var content) ||
                    !figmaNode.TryFindNode<RectTransform>(args, ViewportKey, out var viewport))
                {
                    return figmaNode;
                }

                var scrollRect = figmaNode.gameObject.AddComponent<ScrollRect>();
                scrollRect.inertia = false;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.scrollSensitivity = 10f;

                if (figmaNode.TryFindOptionalNode<Scrollbar>(HorizontalScrollbarKey, out var horizontalScrollbar))
                {
                    scrollRect.horizontalScrollbar = horizontalScrollbar;
                    scrollRect.horizontalScrollbarSpacing = -3; // TODO: auto calculate
                }

                if (figmaNode.TryFindOptionalNode<Scrollbar>(VerticalScrollbarKey, out var verticalScrollbar))
                {
                    scrollRect.verticalScrollbar = verticalScrollbar;
                    scrollRect.verticalScrollbarSpacing = -3; // TODO: auto calculate 
                }

                if (instanceNode.mainComponent != null &&
                    instanceNode.mainComponent.TryGetPluginData(out var pluginData))
                {
                    var componentData = pluginData.GetComponentDataAs<ScrollViewComponentData>();
                    if (componentData != null)
                    {
                        scrollRect.horizontalScrollbarVisibility = componentData.horizontalVisibility;
                        scrollRect.verticalScrollbarVisibility = componentData.verticalVisibility;
                    }
                }

                scrollRect.content = content;
                scrollRect.viewport = viewport;

                var contentSizeFitter = scrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            return figmaNode;
        }

        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            selector = "";
            return false;
        }
    }
}