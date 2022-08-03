using System;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ScrollviewComponentConverter : ComponentConverter
    {
        private const string ContentKey = "@Content";
        private const string ViewportKey = "@Viewport";
        private const string HorizontalScrollbarKey = "@ScrollbarHorizontal";
        private const string VerticalScrollbarKey = "@ScrollbarVertical";
        
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Scrollview";
        }
        
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode instanceNode, NodeConvertArgs args)
        {
            var nodeObject = base.Convert(parentObject, instanceNode, args);
            
            if (nodeObject != null)
            {
                var scrollrect = nodeObject.gameObject.AddComponent<ScrollRect>();

                var content = nodeObject.Find(x => x.bindingKey == ContentKey);
                if (content == null)
                    throw new ArgumentException($"Scrollview content node could not be found. Did you set '{ContentKey}' as binding key?");
                
                var viewport = nodeObject.Find(x => x.bindingKey == ViewportKey);
                if (viewport == null)
                    throw new ArgumentException($"Scrollview viewport node could not be found. Did you set '{ViewportKey}' as binding key?");


                var mask = viewport.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;
                
                var horizontalScrollbar = nodeObject.Find(x => x.bindingKey == HorizontalScrollbarKey);
                var verticalScrollbar = nodeObject.Find(x => x.bindingKey == VerticalScrollbarKey);
                
                if (horizontalScrollbar != null)
                {
                    scrollrect.horizontalScrollbar = horizontalScrollbar.rectTransform.GetComponent<Scrollbar>();
                    scrollrect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                    scrollrect.horizontalScrollbarSpacing = -3;
                }
                if (verticalScrollbar != null)
                {
                    scrollrect.verticalScrollbar = verticalScrollbar.rectTransform.GetComponent<Scrollbar>();
                    scrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                    scrollrect.verticalScrollbarSpacing = -3;
                }
                
                scrollrect.content = content.rectTransform;
                scrollrect.viewport = viewport.rectTransform;

                //TODO: Think about how the scrollview will behave?
                var verticalLayoutGroup = scrollrect.content.gameObject.AddComponent<VerticalLayoutGroup>();
                var contentSizeFitter = scrollrect.content.gameObject.AddComponent<ContentSizeFitter>();
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