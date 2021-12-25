using Unity.VectorGraphics;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeObject Convert(VectorNode node, NodeConvertArgs args)
        {
            var nodeObject =  NodeObject.NewNodeObject(node, args);

            var absoluteBoundingBox = node.absoluteBoundingBox;
            var width = absoluteBoundingBox.width;
            var height = absoluteBoundingBox.height;

            // Override width and height if size property is exist.
            if (node.size != null)
            {
                width = node.size.x;
                height = node.size.y;
            }
            
            if (args.file.TryGetGraphic(node.id, out var sprite))
            {
                var svgImage = nodeObject.gameObject.AddComponent<SVGImage>();
                svgImage.sprite = sprite;
                    
                width = svgImage.sprite.rect.width;
                height = svgImage.sprite.rect.height;
            }
            
            nodeObject.rectTransform.anchorMin = new Vector2(0, 1);
            nodeObject.rectTransform.anchorMax = new Vector2(0, 1);
            nodeObject.rectTransform.pivot = new Vector2(0, 1);
            nodeObject.rectTransform.localPosition = new Vector3(absoluteBoundingBox.x, -absoluteBoundingBox.y);
            nodeObject.rectTransform.sizeDelta = new Vector2(width, height);

            return nodeObject;
        }
        
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }
    }
}