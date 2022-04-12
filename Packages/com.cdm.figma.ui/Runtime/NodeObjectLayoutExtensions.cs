using UnityEngine;

namespace Cdm.Figma.UI
{
    public static class NodeObjectLayoutExtensions
    {
        public static void SetLayoutConstraints(this NodeObject nodeObject, Vector2 parentSize)
        {
            //anchors:
            //min x = left
            //min y = bottom
            //max x = 100-right
            //max y = 100-top

            INodeLayout nodeLayout = (INodeLayout)nodeObject.node;
            INodeTransform nodeTransform = (INodeTransform)nodeObject.node;
            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;
            var anchorMin = nodeObject.rectTransform.anchorMin;
            var anchorMax = nodeObject.rectTransform.anchorMax;

            var position = nodeTransform.GetPosition();
            var positionX = position.x;
            var positionY = position.y;

            if (nodeObject.node.type is NodeType.Group and not NodeType.Frame)
            {
                var parentWidth = parentSize.x;
                var parentHeight = parentSize.y;
                var nodeLeft = positionX < 0 ? positionX * -1 : positionX;
                var nodeRight = parentWidth - (nodeLeft + nodeTransform.size.x);
                if (nodeRight < 0)
                {
                    nodeRight *= -1;
                }

                var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                if (nodeBottom < 0)
                {
                    nodeBottom *= -1;
                }

                var topPercentage = nodeTop / parentHeight;
                topPercentage = 1f - topPercentage;
                var bottomPercentage = nodeBottom / parentHeight;
                var leftPercentage = nodeLeft / parentWidth;
                var rightPercentage = nodeRight / parentWidth;
                rightPercentage = 1f - rightPercentage;
                
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
                    var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = nodeTop / parentHeight;
                    topPercentage = 1f - float.Parse(topPercentage.ToString("F2"));
                    var bottomPercentage = nodeBottom / parentHeight;
                    bottomPercentage = float.Parse(bottomPercentage.ToString("F2"));
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
                    var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = nodeTop / parentHeight;
                    topPercentage = 1f - float.Parse(topPercentage.ToString("F2"));
                    var bottomPercentage = nodeBottom / parentHeight;
                    bottomPercentage = float.Parse(bottomPercentage.ToString("F2"));
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
                    var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
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
                    var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = nodeTop / parentHeight;
                    topPercentage = 1f - float.Parse(topPercentage.ToString("F2"));
                    var bottomPercentage = nodeBottom / parentHeight;
                    bottomPercentage = float.Parse(bottomPercentage.ToString("F2"));
                    anchorMin = new Vector2(0f, bottomPercentage);
                    anchorMax = new Vector2(1f, topPercentage);
                }
            }
            else if (constraintX == Horizontal.Scale)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = positionX < 0 ? positionX * -1 : positionX;
                var nodeRight = parentWidth - (nodeLeft + nodeTransform.size.x);
                if (nodeRight < 0)
                {
                    nodeRight *= -1;
                }

                var leftPercentage = nodeLeft / parentWidth;
                leftPercentage = float.Parse(leftPercentage.ToString("F2"));
                var rightPercentage = nodeRight / parentWidth;
                rightPercentage = 1f - float.Parse(rightPercentage.ToString("F2"));
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
                    var nodeTop = positionY < 0 ? positionY * -1 : positionY;
                    var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                    if (nodeBottom < 0)
                    {
                        nodeBottom *= -1;
                    }

                    var topPercentage = nodeTop / parentHeight;
                    topPercentage = 1f - float.Parse(topPercentage.ToString("F"));
                    var bottomPercentage = nodeBottom / parentHeight;
                    bottomPercentage = float.Parse(bottomPercentage.ToString("F"));
                    anchorMin = new Vector2(leftPercentage, bottomPercentage);
                    anchorMax = new Vector2(rightPercentage, topPercentage);
                }
            }

            nodeObject.rectTransform.anchorMin = anchorMin;
            nodeObject.rectTransform.anchorMax = anchorMax;
            nodeObject.rectTransform.offsetMin = new Vector2(0, 0);
            nodeObject.rectTransform.offsetMax = new Vector2(0, 0);
        }
    }
}