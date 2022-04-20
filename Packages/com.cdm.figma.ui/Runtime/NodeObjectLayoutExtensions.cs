using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class NodeObjectLayoutExtensions
    {
        public static void SetLayoutConstraints(this NodeObject nodeObject, INodeTransform parentTransform)
        {
            //anchors:
            //min x = left
            //min y = bottom
            //max x = 100-right
            //max y = 100-top
            INodeLayout nodeLayout = (INodeLayout) nodeObject.node;
            INodeTransform nodeTransform = (INodeTransform) nodeObject.node;
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
            
            if (nodeObject.node.type is NodeType.Group and not NodeType.Frame)
            {
                var parentWidth = parentSize.x;
                var parentHeight = parentSize.y;
                var nodeLeft = positionX < 0 ? positionX * -1 : positionX;
                var nodeRight = parentWidth - (positionX + nodeTransform.size.x);
                if (nodeRight < 0)
                {
                    nodeRight *= -1;
                }

                var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                var nodeBottom = parentHeight - (-1 * positionY + nodeTransform.size.y);
                if (nodeBottom < 0)
                {
                    nodeBottom *= -1;
                }

                var topPercentage = 1f - nodeTop / parentHeight;
                var bottomPercentage = nodeBottom / parentHeight;
                var leftPercentage = nodeLeft / parentWidth;
                var rightPercentage = 1f - nodeRight / parentWidth;

                anchorMin = new Vector2(leftPercentage, bottomPercentage);
                anchorMax = new Vector2(rightPercentage, topPercentage);
                nodeObject.rectTransform.anchorMin = anchorMin;
                nodeObject.rectTransform.anchorMax = anchorMax;
                nodeObject.rectTransform.offsetMin = new Vector2(0, 0);
                nodeObject.rectTransform.offsetMax = new Vector2(0, 0);
                return;
            }

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
                    var nodeTop = positionY < 0 ? -1*positionY : positionY;
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
            //nodeObject.rectTransform.offsetMin = new Vector2(0, 0);
            //nodeObject.rectTransform.offsetMax = new Vector2(0, 0);
        }

        public static void AdjustPosition(this NodeObject nodeObject, Vector2 parentSize)
        {
            INodeLayout nodeLayout = (INodeLayout) nodeObject.node;
            INodeTransform nodeTransform = (INodeTransform) nodeObject.node;
            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;

            if (constraintX == Horizontal.Center)
            {
                nodeObject.rectTransform.anchoredPosition =
                    new Vector3(
                        (nodeTransform.relativeTransform.GetPosition().x - parentSize.x / 2),
                        nodeObject.rectTransform.anchoredPosition.y, nodeObject.rectTransform.position.z);
            }

            if (constraintX == Horizontal.Right)
            {
                nodeObject.rectTransform.position =
                    new Vector3(-1 * (parentSize.x - nodeObject.rectTransform.position.x),
                        nodeObject.rectTransform.position.y, nodeObject.rectTransform.position.z);
            }

            if (constraintX == Horizontal.LeftRight)
            {
                float left = 0, right = 0;
                left = nodeTransform.relativeTransform.GetPosition().x;
                right = parentSize.x - (left + nodeTransform.size.x);
                nodeObject.rectTransform.offsetMin = new Vector2(left, nodeObject.rectTransform.offsetMin.y);
                nodeObject.rectTransform.offsetMax = new Vector2(-right, nodeObject.rectTransform.offsetMax.y);
            }

            if (constraintX == Horizontal.Scale)
            {
                nodeObject.rectTransform.offsetMin = new Vector2(0, nodeObject.rectTransform.offsetMin.y);
                nodeObject.rectTransform.offsetMax = new Vector2(0, nodeObject.rectTransform.offsetMax.y);
            }

            if (constraintY == Vertical.Center)
            {
                nodeObject.rectTransform.anchoredPosition =
                    new Vector3(nodeObject.rectTransform.anchoredPosition.x,
                        -1 * (-1*nodeTransform.relativeTransform.GetPosition().y - parentSize.y / 2),
                        nodeObject.rectTransform.position.z);
            }

            if (constraintY == Vertical.Bottom)
            {
                nodeObject.rectTransform.position =
                    new Vector3(nodeObject.rectTransform.position.x,
                        (parentSize.y + nodeObject.rectTransform.position.y), nodeObject.rectTransform.position.z);
            }

            if (constraintY == Vertical.TopBottom)
            {
                float top = 0, bottom = 0;
                top = nodeTransform.relativeTransform.GetPosition().y;
                bottom = parentSize.y - (-top + nodeTransform.size.y);
                nodeObject.rectTransform.offsetMin = new Vector2(nodeObject.rectTransform.offsetMin.x, bottom);
                nodeObject.rectTransform.offsetMax = new Vector2(nodeObject.rectTransform.offsetMax.x, top);
            }

            if (constraintY == Vertical.Scale)
            {
                nodeObject.rectTransform.offsetMin = new Vector2(nodeObject.rectTransform.offsetMin.x, 0);
                nodeObject.rectTransform.offsetMax = new Vector2(nodeObject.rectTransform.offsetMax.x, 0);
            }
        }
    }
}