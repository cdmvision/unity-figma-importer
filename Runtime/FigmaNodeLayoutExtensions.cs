using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class FigmaNodeLayoutExtensions
    {
        public static void SetLayoutConstraints(this FigmaNode nodeObject, INodeTransform parentTransform)
        {
            // Anchors:
            // min x = left
            // min y = bottom
            // max x = 100-right
            // max y = 100-top
            var targetNode = nodeObject.referenceNode ?? nodeObject.node;
            var nodeLayout = (INodeLayout)targetNode;
            var nodeTransform = (INodeTransform)targetNode;

            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;
            var anchorMin = nodeObject.rectTransform.anchorMin;
            var anchorMax = nodeObject.rectTransform.anchorMax;

            var position = nodeTransform.GetPosition();
            var positionX = position.x;
            var positionY = position.y;
            var nodeWidth = nodeTransform.size.x;
            var nodeHeight = nodeTransform.size.y;

            var parentSize = parentTransform.size;

            if (constraintX == Horizontal.Center)
            {
                if (constraintY == Vertical.Center)
                {
                    anchorMin = new Vector2(0.5f, 0.5f);
                    anchorMax = new Vector2(0.5f, 0.5f);
                }
                else if (constraintY == Vertical.Top)
                {
                    anchorMin = new Vector2(0.5f, 1f);
                    anchorMax = new Vector2(0.5f, 1f);
                }
                else if (constraintY == Vertical.Bottom)
                {
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 0f);
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 1f);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parentSize.y;
                    var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                    var nodeBottom = parentHeight - (-1 * positionY + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = 1f - nodeTop / parentHeight;
                    var bottomPercentage = nodeBottom / parentHeight;
                    anchorMin = new Vector2(0.5f, bottomPercentage);
                    anchorMax = new Vector2(0.5f, topPercentage);
                }
            }
            else if (constraintX == Horizontal.Left)
            {
                if (constraintY == Vertical.Center)
                {
                    anchorMin = new Vector2(0f, 0.5f);
                    anchorMax = new Vector2(0f, 0.5f);
                }
                else if (constraintY == Vertical.Top)
                {
                    anchorMin = new Vector2(0f, 1f);
                    anchorMax = new Vector2(0f, 1f);
                }
                else if (constraintY == Vertical.Bottom)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(0f, 0f);
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(0f, 1f);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parentSize.y;
                    var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                    var nodeBottom = parentHeight - (-1 * positionY + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = 1f - nodeTop / parentHeight;
                    var bottomPercentage = nodeBottom / parentHeight;
                    anchorMin = new Vector2(0f, bottomPercentage);
                    anchorMax = new Vector2(0f, topPercentage);
                }
            }
            else if (constraintX == Horizontal.Right)
            {
                if (constraintY == Vertical.Center)
                {
                    anchorMin = new Vector2(1f, 0.5f);
                    anchorMax = new Vector2(1f, 0.5f);
                }
                else if (constraintY == Vertical.Top)
                {
                    anchorMin = new Vector2(1f, 1f);
                    anchorMax = new Vector2(1f, 1f);
                }
                else if (constraintY == Vertical.Bottom)
                {
                    anchorMin = new Vector2(1f, 0f);
                    anchorMax = new Vector2(1f, 0f);
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(1f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parentSize.y;
                    var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                    var nodeBottom = parentHeight - (-1 * positionY + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = nodeTop / parentHeight;
                    topPercentage = 1f - topPercentage;
                    var bottomPercentage = nodeBottom / parentHeight;
                    anchorMin = new Vector2(1f, bottomPercentage);
                    anchorMax = new Vector2(1f, topPercentage);
                }
            }
            else if (constraintX == Horizontal.LeftRight)
            {
                if (constraintY == Vertical.Center)
                {
                    anchorMin = new Vector2(0f, 0.5f);
                    anchorMax = new Vector2(1f, 0.5f);
                }
                else if (constraintY == Vertical.Top)
                {
                    anchorMin = new Vector2(0f, 1f);
                    anchorMax = new Vector2(1f, 1f);
                }
                else if (constraintY == Vertical.Bottom)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(1f, 0f);
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parentSize.y;
                    var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                    var nodeBottom = parentHeight - (-1 * positionY + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = 1f - nodeTop / parentHeight;
                    var bottomPercentage = nodeBottom / parentHeight;
                    anchorMin = new Vector2(0f, bottomPercentage);
                    anchorMax = new Vector2(1f, topPercentage);
                }
            }
            else if (constraintX == Horizontal.Scale)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = positionX < 0 ? positionX * -1 : positionX;
                var nodeRight = parentWidth - (nodeLeft + nodeWidth);
                if (nodeRight < 0)
                {
                    nodeRight *= -1;
                }

                var leftPercentage = nodeLeft / parentWidth;
                var rightPercentage = 1f - nodeRight / parentWidth;
                if (constraintY == Vertical.Center)
                {
                    anchorMin = new Vector2(leftPercentage, 0.5f);
                    anchorMax = new Vector2(rightPercentage, 0.5f);
                }
                else if (constraintY == Vertical.Top)
                {
                    anchorMin = new Vector2(leftPercentage, 1f);
                    anchorMax = new Vector2(rightPercentage, 1f);
                }
                else if (constraintY == Vertical.Bottom)
                {
                    anchorMin = new Vector2(leftPercentage, 0f);
                    anchorMax = new Vector2(rightPercentage, 0f);
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(leftPercentage, 0f);
                    anchorMax = new Vector2(rightPercentage, 1f);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parentSize.y;
                    var nodeTop = positionY < 0 ? -1 * positionY : positionY;
                    var nodeBottom = parentHeight - (nodeTop + nodeHeight);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = 1f - nodeTop / parentHeight;
                    var bottomPercentage = nodeBottom / parentHeight;
                    anchorMin = new Vector2(leftPercentage, bottomPercentage);
                    anchorMax = new Vector2(rightPercentage, topPercentage);
                }
            }

            nodeObject.rectTransform.anchorMin = anchorMin;
            nodeObject.rectTransform.anchorMax = anchorMax;
        }

        private static void AdjustAnchoredPosition(LayoutConstraint constraint, Vector2 parentSize,
            RectTransform rectTransform, INodeTransform nodeTransform)
        {
            var size = nodeTransform.size;
            var relativePosition = nodeTransform.relativeTransform.GetPosition();

            var anchoredPosition = rectTransform.anchoredPosition;

            if (constraint.horizontal == Horizontal.Center)
            {
                anchoredPosition.x = relativePosition.x - parentSize.x * 0.5f + size.x * 0.5f;
            }

            if (constraint.vertical == Vertical.Center)
            {
                anchoredPosition.y = relativePosition.y + parentSize.y * 0.5f - size.y * 0.5f;
            }

            rectTransform.anchoredPosition = anchoredPosition;
        }

        private static void AdjustPosition(LayoutConstraint constraint, Vector2 parentSize,
            RectTransform rectTransform)
        {
            var position = rectTransform.position;

            // There is no need any modification for Horizontal.Left and Vertical.Top.
            
            if (constraint.horizontal == Horizontal.Right)
            {
                position.x = -1 * (parentSize.x - position.x);
            }

            if (constraint.vertical == Vertical.Bottom)
            {
                position.y = parentSize.y + position.y;
            }

            rectTransform.position = position;
        }

        private static void AdjustOffsetMinMax(LayoutConstraint constraint, Vector2 parentSize,
            RectTransform rectTransform, INodeTransform nodeTransform)
        {
            var offsetMin = rectTransform.offsetMin;
            var offsetMax = rectTransform.offsetMax;
            var relativePosition = nodeTransform.relativeTransform.GetPosition();

            if (constraint.horizontal == Horizontal.LeftRight)
            {
                offsetMin.x = relativePosition.x;
                offsetMax.x = -(parentSize.x - (relativePosition.x + nodeTransform.size.x));
            }

            if (constraint.vertical == Vertical.TopBottom)
            {
                offsetMin.y = parentSize.y - (-relativePosition.y + nodeTransform.size.y);
                offsetMax.y = relativePosition.y;
            }

            if (constraint.horizontal == Horizontal.Scale)
            {
                offsetMin.x = 0;
                offsetMax.x = 0;
            }

            if (constraint.vertical == Vertical.Scale)
            {
                offsetMin.y = 0;
                offsetMax.y = 0;
            }

            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }

        public static void AdjustRectTransform(this FigmaNode nodeObject, Vector2 parentSize)
        {
            var targetNode = nodeObject.referenceNode ?? nodeObject.node;
            var nodeLayout = (INodeLayout)targetNode;
            var nodeTransform = (INodeTransform)targetNode;
            var rectTransform = nodeObject.rectTransform;

            // Center
            AdjustAnchoredPosition(nodeLayout.constraints, parentSize, rectTransform, nodeTransform);
            
            // Left, Top, Right, Bottom
            AdjustPosition(nodeLayout.constraints, parentSize, rectTransform);
            
            // LeftRight, TopBottom, Scale
            AdjustOffsetMinMax(nodeLayout.constraints, parentSize, rectTransform, nodeTransform);
        }
    }
}