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
            vectorNodeObject.rectTransform.position = new Vector3(vectorNode.relativeTransform.GetPosition().x,
                vectorNode.relativeTransform.GetPosition().y * -1, 0);
        }

        private static void SetSize(VectorNode vectorNode, NodeObject vectorNodeObject)
        {
            vectorNodeObject.rectTransform.sizeDelta = new Vector2(vectorNode.size.x, vectorNode.size.y);
        }

        private static void SetTransformOrigin(NodeObject nodeObject)
        {
            nodeObject.rectTransform.pivot = new Vector2(0, 1);
        }

        private static void SetRotation(Node node, NodeObject nodeObject)
        {
            VectorNode vectorNode = (VectorNode) node;
            var relativeTransform = vectorNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            rotation = float.Parse(rotation.ToString("F2"));
            if (rotation != 0.0f)
            {
                nodeObject.rectTransform.transform.eulerAngles = new Vector3(0, 0, rotation * -1);
            }
        }
    }
}