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
                scrollRect.content = content;
                scrollRect.viewport = viewport;
                scrollRect.inertia = false;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.scrollSensitivity = 10f;

                // Make items to fill container.
                var verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
                if (verticalLayoutGroup != null)
                {
                    verticalLayoutGroup.childControlWidth = true;
                    verticalLayoutGroup.childForceExpandWidth = true;
                }
                
                var horizontalLayoutGroup = content.GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayoutGroup != null)
                {
                    horizontalLayoutGroup.childControlHeight = true;
                    horizontalLayoutGroup.childForceExpandHeight = true;
                }
                
                var contentSizeFitter = scrollRect.content.gameObject.GetOrAddComponent<ContentSizeFitter>();
                
                if (figmaNode.TryFindOptionalNode<Scrollbar>(HorizontalScrollbarKey, out var horizontalScrollbar))
                {
                    scrollRect.horizontalScrollbar = horizontalScrollbar;
                    scrollRect.verticalScrollbarSpacing = horizontalScrollbar.GetComponent<RectTransform>().sizeDelta.y;
                    
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }

                if (figmaNode.TryFindOptionalNode<Scrollbar>(VerticalScrollbarKey, out var verticalScrollbar))
                {
                    scrollRect.verticalScrollbar = verticalScrollbar;
                    scrollRect.horizontalScrollbarSpacing = verticalScrollbar.GetComponent<RectTransform>().sizeDelta.x;
                    
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
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