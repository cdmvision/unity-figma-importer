using Unity.VectorGraphics;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeObject Convert(NodeObject parentObject, VectorNode vectorNode, NodeConvertArgs args)
        {
            var nodeObject =  NodeObject.NewNodeObject(vectorNode, args);
            
            SetTransformOrigin(nodeObject);
            SetSize(vectorNode, nodeObject);
            SetRotation(vectorNode, nodeObject);
            HandleConstraints(nodeObject, vectorNode);
            SetPosition(vectorNode, nodeObject);

            if (args.file.TryGetGraphic(vectorNode.id, out var sprite))
            {
                var svgImage = nodeObject.gameObject.AddComponent<SVGImage>();
                svgImage.sprite = sprite;
            }
            
            NodeConverterHelper.ConvertEffects(nodeObject, vectorNode.effects);
            
            return nodeObject;
        }
        
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (VectorNode) node, args);
        }
        
        private static void SetPosition(VectorNode vectorNode, NodeObject vectorNodeObject)
        {
            vectorNodeObject.rectTransform.position = new Vector3(vectorNode.relativeTransform.GetPosition().x,
                vectorNode.relativeTransform.GetPosition().y*-1,0);
        }

        private static void SetSize(VectorNode vectorNode, NodeObject vectorNodeObject)
        {
            vectorNodeObject.rectTransform.sizeDelta = new Vector2(vectorNode.size.x, vectorNode.size.y);
        }

        private static void SetTransformOrigin(NodeObject nodeObject)
        {
            nodeObject.rectTransform.pivot = new Vector2(0,1);
        }
        
        private static void SetRotation(Node node, NodeObject nodeObject)
        {
            VectorNode vectorNode = (VectorNode) node;
            var relativeTransform = vectorNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            rotation = float.Parse(rotation.ToString("F2"));
            if (rotation != 0.0f)
            {
                nodeObject.rectTransform.transform.eulerAngles = new Vector3(0, 0, rotation*-1);
            }
        }
        
        private static void HandleConstraints(NodeObject nodeObject, INodeLayout nodeLayout)
        {
            //anchors:
            //min x = left
            //min y = bottom
            //max x = 100-right
            //max y = 100-top
            
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
                //TODO: implement top-bottom and scale
                else if (constraintY == Vertical.TopBottom)
                {
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 1f);
                }
                else if (constraintY == Vertical.Scale)
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