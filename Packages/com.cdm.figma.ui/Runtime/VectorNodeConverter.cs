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

        public NodeSpriteGenerator.SpriteOptions spriteOptions { get; set; } = new NodeSpriteGenerator.SpriteOptions()
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            sampleCount = 8,
            textureSize = 1024
        };
    }

    public abstract class VectorNodeConverter<TNode, TFigmaNode> : NodeConverter<TNode> 
        where TNode : VectorNode 
        where TFigmaNode : FigmaNode
    {
        protected FigmaNode Convert(FigmaNode parentObject, TNode vectorNode, NodeConvertArgs args,
            VectorConvertArgs vectorConvertArgs)
        {
            var figmaNode = args.importer.CreateFigmaNode<TFigmaNode>(vectorNode);
            figmaNode.SetTransform(vectorNode);

            // Every vector's parent will ALWAYS be INodeTransform
            figmaNode.SetLayoutConstraints((INodeTransform)vectorNode.parent);

            GenerateStyles(figmaNode, vectorNode, args, vectorConvertArgs);

            figmaNode.ApplyStyles();
            return figmaNode;
        }

        protected override FigmaNode Convert(FigmaNode parentObject, TNode vectorNode, NodeConvertArgs args)
        {
            return Convert(parentObject, vectorNode, args, new VectorConvertArgs());
        }

        private void GenerateStyles(FigmaNode nodeObject, TNode vectorNode, NodeConvertArgs args,
            VectorConvertArgs vectorConvertArgs)
        {
            if (vectorConvertArgs.generateSprite && (vectorNode.fills.Any() || vectorNode.strokes.Any()))
            {
                var style = new ImageStyle();
                if (vectorConvertArgs.sourceSprite == null)
                {
                    if (!args.importer.generatedAssets.TryGet<Sprite>(vectorNode.id, out var sprite))
                    {
                        sprite = NodeSpriteGenerator.GenerateSprite(
                            vectorNode, SpriteGenerateType.Path, vectorConvertArgs.spriteOptions);
                        if (sprite != null)
                        {
                            args.importer.generatedAssets.Add(vectorNode.id, sprite);
                            args.importer.generatedAssets.Add(vectorNode.id, sprite.texture);
                        }
                    }

                    vectorConvertArgs.sourceSprite = sprite;
                }

                style.componentEnabled.enabled = true;
                style.componentEnabled.value = vectorConvertArgs.sourceSprite != null;

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

    public class VectorNodeConverter : VectorNodeConverter<VectorNode, FigmaNode>
    {
    }
}