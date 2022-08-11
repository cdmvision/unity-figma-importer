using System.Linq;
using Cdm.Figma.UI.Styles;
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

            // Any vector's parent will ALWAYS be INodeTransform
            nodeObject.SetLayoutConstraints((INodeTransform)vectorNode.parent);

            GenerateStyles(nodeObject, vectorNode, args, vectorConvertArgs);

            nodeObject.ApplyStyles();
            return nodeObject;
        }

        protected override NodeObject Convert(NodeObject parentObject, TNode vectorNode, NodeConvertArgs args)
        {
            return Convert(parentObject, vectorNode, args, new VectorConvertArgs());
        }

        private void GenerateStyles(NodeObject nodeObject, TNode vectorNode, NodeConvertArgs args,
            VectorConvertArgs vectorConvertArgs)
        {
            if (vectorConvertArgs.generateSprite && (vectorNode.fills.Any() || vectorNode.strokes.Any()))
            {
                var style = new ImageStyle();
                if (vectorConvertArgs.sourceSprite == null)
                {
                    vectorConvertArgs.sourceSprite =
                        VectorImageUtils.CreateSpriteFromPath(vectorNode, vectorConvertArgs.spriteOptions);
                }

                style.componentEnabled.enabled = true;
                style.componentEnabled.value = vectorNode.fills.Any(fill => fill.visible) ||
                                               vectorNode.strokes.Any(stroke => stroke.visible);

                style.sprite.enabled = true;
                style.sprite.value = vectorConvertArgs.sourceSprite;

                style.imageType.enabled = true;
                style.imageType.value = vectorNode is INodeRect ? Image.Type.Sliced : Image.Type.Simple;

                style.color.enabled = true;
                style.color.value = new UnityEngine.Color(1f, 1f, 1f, vectorNode.opacity);
                nodeObject.styles.Add(style);
            }

            NodeConverterHelper.GenerateEffectsStyles(nodeObject, vectorNode.effects);
        }
    }

    public class VectorNodeConverter : VectorNodeConverter<VectorNode>
    {
    }
}