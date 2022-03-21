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

            nodeObject.SetLayoutConstraints(((INodeTransform)vectorNode.parent).size);
            if ((vectorNode.fills.Any() || vectorNode.strokes.Any()) && vectorNode.type != NodeType.Text)
            {
                var options = new VectorImageUtils.SpriteOptions()
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    sampleCount = 8,
                    textureSize = 1024
                };

                Sprite sprite = null;
                if (vectorNode is RectangleNode)
                {
                    // Use scale only gor manually generated image.
                    // Mirroring value is baked for pre-rendered graphics by Figma.
                    nodeObject.SetTransform(vectorNode, TransformType.Relative);
                    nodeObject.SetSize(vectorNode, TransformType.Relative);
                    sprite = VectorImageUtils.CreateSpriteFromRect(vectorNode, options);
                }
                else
                {
                    nodeObject.SetTransform(vectorNode, TransformType.Absolute);
                    nodeObject.SetSize(vectorNode, TransformType.Absolute);
                    
                    if (args.file.TryGetGraphic(vectorNode.id, out var svg))
                    {
                        sprite = VectorImageUtils.CreateSpriteFromSvg(vectorNode, svg, options);    
                    }
                    else
                    {
                        Debug.LogWarning($"Graphic could not be found: {vectorNode.id}");
                    }
                }

                var image = nodeObject.gameObject.AddComponent<Image>();
                image.sprite = sprite;
                image.type = vectorNode is INodeRect ? Image.Type.Sliced : Image.Type.Simple;
                image.color = new UnityEngine.Color(1f, 1f, 1f, vectorNode.opacity);
            }

            
            NodeConverterHelper.ConvertEffects(nodeObject, vectorNode.effects);

            return nodeObject;
        }

        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (VectorNode)node, args);
        }
    }
}