using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeObject Convert(NodeObject parentObject, GroupNode groupNode, NodeConvertArgs args)
        {
            var groupNodeObject = NodeObject.NewNodeObject(groupNode, args);
            
            SetTransformOrigin(groupNodeObject);
            SetSize(groupNode, groupNodeObject);
            SetPosition(groupNode, groupNodeObject);
            SetRotation(groupNode, groupNodeObject);
            HandleConstraints(groupNodeObject, groupNode);
            
            if (args.file.TryGetGraphic(groupNode.id, out var sprite))
            {
                var svgImage = groupNodeObject.gameObject.AddComponent<SVGImage>();
                svgImage.sprite = sprite;
            }

            if (groupNode.layoutMode != LayoutMode.None)
            {
                HandleAutoLayout(groupNode, groupNodeObject);
                //TODO: Handle Axis Sizing
            }

            var children = groupNode.children;
            if (children != null)
            {
                for (var child = 0; child < children.Length; child++)
                {
                    if (args.importer.TryConvertNode(groupNodeObject, children[child], args, out var childElement))
                    {
                        if (groupNode.layoutMode != LayoutMode.None)
                        {
                            childElement.gameObject.AddComponent<LayoutElement>();
                            //TODO: Handle resizing
                        }
                        
                        if (childElement != groupNodeObject)
                        {
                            childElement.rectTransform.SetParent(groupNodeObject.rectTransform);
                        }
                    }
                }
            }

            return groupNodeObject;
        }
        
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (GroupNode) node, args);
        }

        private static void HandleAutoLayout(GroupNode groupNode, NodeObject groupNodeObject)
        {
            var layoutMode = groupNode.layoutMode;
            if (layoutMode == LayoutMode.Horizontal)
            {
                groupNodeObject.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else
            {
                groupNodeObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            
            //TODO: FINISH UP AUTO LAYOUT 
            /*
            if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
            {
                
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
            {
                
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
            {
                
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
            {
                
            }

            if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
            {
                
            }
            else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
            {
                
            }
            else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
            {
                
            }
            */
        }

        private static void SetPosition(GroupNode groupNode, NodeObject groupNodeObject)
        {
            groupNodeObject.rectTransform.position = new Vector3(groupNode.relativeTransform.GetPosition().x,
                groupNode.relativeTransform.GetPosition().y*-1,0);
        }

        private static void SetSize(GroupNode groupNode, NodeObject groupNodeObject)
        {
            groupNodeObject.rectTransform.sizeDelta = new Vector2(groupNode.size.x, groupNode.size.y);
        }

        private static void SetTransformOrigin(NodeObject nodeObject)
        {
            nodeObject.rectTransform.pivot = new Vector2(0,1);
        }
        
        private static void SetRotation(Node node, NodeObject nodeObject)
        {
            GroupNode groupNode = (GroupNode) node;
            var relativeTransform = groupNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            rotation = float.Parse(rotation.ToString("F2"));
            if (rotation != 0.0f)
            {
                nodeObject.rectTransform.transform.eulerAngles = new Vector3(0, 0, rotation*-1);
            }
        }
        
        private static void HandleConstraints(NodeObject nodeObject, INodeLayout nodeLayout)
        {
            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;
            var anchorMin = nodeObject.rectTransform.anchorMin;
            var anchorMax = nodeObject.rectTransform.anchorMax;
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
                else if (constraintY is Vertical.TopBottom or Vertical.Scale)
                {
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 1f);
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
                else if (constraintY is Vertical.TopBottom or Vertical.Scale)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(0f, 1f);
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
                else if (constraintY is Vertical.TopBottom or Vertical.Scale)
                {
                    anchorMin = new Vector2(1f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                }
            }
            else if (constraintX is Horizontal.LeftRight or Horizontal.Scale)
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
                else if (constraintY is Vertical.TopBottom or Vertical.Scale)
                {
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                }
            }

            nodeObject.rectTransform.anchorMin = anchorMin;
            nodeObject.rectTransform.anchorMax = anchorMax;
        }
    }
}