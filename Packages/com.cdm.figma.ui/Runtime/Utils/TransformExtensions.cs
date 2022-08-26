using UnityEngine;

namespace Cdm.Figma.UI.Utils
{
    internal static class TransformExtensions
    {
        public static void CopyTo(this RectTransform rectTransform, RectTransform other)
        {
            rectTransform.ForceUpdateRectTransforms();
            other.pivot = rectTransform.pivot;
            other.position = rectTransform.position;
            other.rotation = rectTransform.rotation;
            other.localScale = rectTransform.localScale;
            other.anchorMin = rectTransform.anchorMin;
            other.anchorMax = rectTransform.anchorMax;
            other.anchoredPosition = rectTransform.anchoredPosition;
            other.sizeDelta = rectTransform.sizeDelta;
            other.offsetMin = rectTransform.offsetMin;
            other.offsetMax = rectTransform.offsetMax;
        }
    }
}