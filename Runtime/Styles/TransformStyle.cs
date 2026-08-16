using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class TransformStyle : StyleWithSetter<TransformStyleSetter>
    {
        public StylePropertyVector2 pivot = new StylePropertyVector2(Vector2.one * 0.5f);
        public StylePropertyVector2 sizeDelta = new StylePropertyVector2(Vector2.zero);
        public StylePropertyVector2 anchoredPosition = new StylePropertyVector2(Vector2.zero);
        public StylePropertyVector2 anchorMin = new StylePropertyVector2(Vector2.zero);
        public StylePropertyVector2 anchorMax = new StylePropertyVector2(Vector2.one);
        public StylePropertyVector2 offsetMin = new StylePropertyVector2(Vector2.zero);
        public StylePropertyVector2 offsetMax = new StylePropertyVector2(Vector2.one);
        public StylePropertyVector3 position = new StylePropertyVector3(Vector3.zero);
        public StylePropertyVector3 scale = new StylePropertyVector3(Vector3.one);
        public StylePropertyQuaternion rotation = new StylePropertyQuaternion(Quaternion.identity);

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (pivot.enabled)
                    rectTransform.pivot = pivot.value;

                if (anchorMin.enabled)
                    rectTransform.anchorMin = anchorMin.value;

                if (anchorMax.enabled)
                    rectTransform.anchorMax = anchorMax.value;

                if (anchoredPosition.enabled)
                    rectTransform.anchoredPosition = anchoredPosition.value;

                if (sizeDelta.enabled)
                    rectTransform.sizeDelta = sizeDelta.value;

                if (offsetMin.enabled)
                    rectTransform.offsetMin = offsetMin.value;

                if (offsetMax.enabled)
                    rectTransform.offsetMax = offsetMax.value;

                if (position.enabled)
                    rectTransform.localPosition = position.value;

                if (scale.enabled)
                    rectTransform.localScale = scale.value;

                if (rotation.enabled)
                    rectTransform.localRotation = rotation.value;
            }
        }

        protected override void MergeTo(Style other, bool force)
        {
            if (other is TransformStyle otherStyle)
            {
                OverwriteProperty(pivot, otherStyle.pivot, force);
                OverwriteProperty(anchorMin, otherStyle.anchorMin, force);
                OverwriteProperty(anchorMax, otherStyle.anchorMax, force);
                OverwriteProperty(anchoredPosition, otherStyle.anchoredPosition, force);
                OverwriteProperty(sizeDelta, otherStyle.sizeDelta, force);
                OverwriteProperty(offsetMin, otherStyle.offsetMin, force);
                OverwriteProperty(offsetMax, otherStyle.offsetMax, force);
                OverwriteProperty(position, otherStyle.position, force);
                OverwriteProperty(scale, otherStyle.scale, force);
                OverwriteProperty(rotation, otherStyle.rotation, force);
            }
        }

        public static TransformStyle GetTransformStyle(RectTransform rectTransform)
        {
            var transformStyle = new TransformStyle();
            transformStyle.pivot.enabled = true;
            transformStyle.pivot.value = rectTransform.pivot;

            transformStyle.sizeDelta.enabled = true;
            transformStyle.sizeDelta.value = rectTransform.sizeDelta;

            transformStyle.anchoredPosition.enabled = true;
            transformStyle.anchoredPosition.value = rectTransform.anchoredPosition;

            transformStyle.anchorMin.enabled = true;
            transformStyle.anchorMin.value = rectTransform.anchorMin;

            transformStyle.anchorMax.enabled = true;
            transformStyle.anchorMax.value = rectTransform.anchorMax;

            transformStyle.offsetMin.enabled = true;
            transformStyle.offsetMin.value = rectTransform.offsetMin;

            transformStyle.offsetMax.enabled = true;
            transformStyle.offsetMax.value = rectTransform.offsetMax;

            transformStyle.position.enabled = true;
            transformStyle.position.value = rectTransform.localPosition;

            transformStyle.scale.enabled = true;
            transformStyle.scale.value = rectTransform.localScale;

            transformStyle.rotation.enabled = true;
            transformStyle.rotation.value = rectTransform.localRotation;
            return transformStyle;
        }
    }
}