using Cdm.Figma.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.Examples
{
    public abstract class ScrollableContent : Content, IFigmaNodeBinder
    {
        protected abstract RectTransform GetContent();

        public void OnBind(FigmaNode node)
        {
            var scrollRect = GetComponentInChildren<ScrollRect>();
            GetContent().SetParent(scrollRect.content);
            scrollRect.horizontal = false;
        }
    }
}