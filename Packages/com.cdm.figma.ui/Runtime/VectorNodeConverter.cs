using System.Linq;
using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeObject Convert(NodeObject parentObject, VectorNode vectorNode, NodeConvertArgs args)
        {
            var nodeObject = NodeObject.NewNodeObject(vectorNode, args);
            SetTransformOrigin(nodeObject);
            SetSize(vectorNode, nodeObject);
            SetRotation(vectorNode, nodeObject);
            SetPosition(vectorNode, nodeObject);

            if ((vectorNode.fills.Count > 0 || vectorNode.strokes.Any()) && vectorNode.type != NodeType.Text)
            {
                var sprite = VectorImageUtils.CreateSprite(vectorNode, new VectorImageUtils.SpriteOptions()
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    sampleCount = 8,
                    textureSize = 1024
                });
                var image = nodeObject.gameObject.AddComponent<Image>();
                image.sprite = sprite;
                image.type = vectorNode is INodeRect ? Image.Type.Sliced : Image.Type.Simple;
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
            var scale = vectorNode.relativeTransform.GetScale();
            var offset = Vector2.Scale((Vector2) vectorNode.size * 0.5f, scale);
            var position = vectorNode.relativeTransform.GetPosition();

            vectorNodeObject.rectTransform.localPosition = new Vector3(position.x + offset.x, position.y - offset.y, 0);
            vectorNodeObject.rectTransform.localScale = new Vector3(scale.x, scale.y, 1);
        }

        private static void SetSize(VectorNode vectorNode, NodeObject vectorNodeObject)
        {
            vectorNodeObject.rectTransform.sizeDelta = new Vector2(vectorNode.size.x, vectorNode.size.y);
        }

        private static void SetTransformOrigin(NodeObject nodeObject)
        {
            nodeObject.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void SetRotation(Node node, NodeObject nodeObject)
        {
            VectorNode vectorNode = (VectorNode) node;
            var relativeTransform = vectorNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            nodeObject.rectTransform.localRotation = relativeTransform.GetRotation();
        }
    }
}