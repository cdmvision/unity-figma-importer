using System.Linq;
using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class VectorConvertArgs
    {
        public Sprite sourceSprite { get; set; }
        public bool generateSprite { get; set; } = true;

        public VectorImageUtils.SpriteOptions spriteOptions { get; set; } = new VectorImageUtils.SpriteOptions()
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            sampleCount = 8,
            textureSize = 1024
        };
    }

    public abstract class VectorNodeConverter<TNode> : NodeConverter<TNode> where TNode : VectorNode
    {
        protected NodeObject Convert(NodeObject parentObject, TNode vectorNode, NodeConvertArgs args,
            VectorConvertArgs vectorConvertArgs)
        {
            var nodeObject = NodeObject.NewNodeObject(vectorNode, args);
            nodeObject.SetTransform(vectorNode);
            //any vector's parent will ALWAYS be INodeTransform
            nodeObject.SetLayoutConstraints((INodeTransform)vectorNode.parent);
            if (vectorConvertArgs.generateSprite && (vectorNode.fills.Any() || vectorNode.strokes.Any()))
            {
                if (vectorConvertArgs.sourceSprite == null)
                {
                    vectorConvertArgs.sourceSprite =
                        VectorImageUtils.CreateSpriteFromPath(vectorNode, vectorConvertArgs.spriteOptions);
                }

                var image = nodeObject.gameObject.AddComponent<Image>();
                image.sprite = vectorConvertArgs.sourceSprite;
                image.type = vectorNode is INodeRect ? Image.Type.Sliced : Image.Type.Simple;
                image.color = new UnityEngine.Color(1f, 1f, 1f, vectorNode.opacity);
            }

            NodeConverterHelper.ConvertEffects(nodeObject, vectorNode.effects);

            return nodeObject;
        }

        protected override NodeObject Convert(NodeObject parentObject, TNode vectorNode, NodeConvertArgs args)
        {
            return Convert(parentObject, vectorNode, args, new VectorConvertArgs());
        }
        
    }

    public class VectorNodeConverter : VectorNodeConverter<VectorNode>
    {
    }
}