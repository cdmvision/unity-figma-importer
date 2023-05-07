using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
using Cdm.Figma.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class VectorConvertArgs
    {
        public Sprite sourceSprite { get; set; }
        public bool generateSprite { get; set; } = true;
    }

    public abstract class VectorNodeConverter<TNode, TFigmaNode> : NodeConverter<TNode>
        where TNode : VectorNode
        where TFigmaNode : FigmaNode
    {
        protected override FigmaNode Convert(FigmaNode parentObject, TNode vectorNode, NodeConvertArgs args)
        {
            var figmaNode = Convert(parentObject, vectorNode, args, new VectorConvertArgs());
            if (figmaNode != null && vectorNode.isMask)
            {
                args.importer.LogWarning("Vector node with mask is not supported.", figmaNode);
            }

            return figmaNode;
        }

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

        private static void GenerateStyles(FigmaNode nodeObject, TNode vectorNode, NodeConvertArgs args,
            VectorConvertArgs vectorConvertArgs)
        {
            if (vectorConvertArgs.generateSprite)
            {
                var sprite = vectorConvertArgs.sourceSprite;
                if (sprite == null)
                {
                    sprite = GenerateSprite(vectorNode, nodeObject, SpriteGenerateType.Path, args);
                    
                    if (sprite != null)
                    {
                        vectorConvertArgs.sourceSprite = sprite;
                    }
                }

                var style = new ImageStyle();
                style.componentEnabled.enabled = true;
                style.componentEnabled.value = vectorConvertArgs.sourceSprite != null;

                style.sprite.enabled = true;
                style.sprite.value = vectorConvertArgs.sourceSprite;

                style.imageType.enabled = true;
                style.imageType.value = sprite.GetImageType();
                nodeObject.styles.Add(style);
            }

            {
                var style = new CanvasGroupStyle();
                style.alpha.enabled = true;
                style.alpha.value = vectorNode.opacity;
                nodeObject.styles.Add(style);
            }

            args.importer.ConvertEffects(nodeObject, vectorNode.effects);
        }
        
        public static Sprite GenerateSprite(SceneNode node, FigmaNode figmaNode, 
            SpriteGenerateType generateType, NodeConvertArgs args)
        {
            if (node is not INodeFill)
                return null;
            
            var nodeId = node.id;
            var spriteOptions = args.importer.spriteOptions;
            
            var fills = ((INodeFill)node).fills;
            var strokes = ((INodeFill)node).strokes;
            
            if (args.overrideNode is INodeFill overrideNodeFill and SceneNode overrideSceneNode)
            {
                nodeId = $"{node.id}_{args.overrideNode.id}";
                spriteOptions.overrideNode = overrideSceneNode;
                
                fills = overrideNodeFill.fills;
                strokes = overrideNodeFill.strokes;
            }
            
            if ((fills != null && fills.Any()) || (strokes != null && strokes.Any()))
            {
                if (!args.importer.generatedAssets.TryGet<Sprite>(nodeId, out var sprite))
                {
                    try
                    {
                        sprite = NodeSpriteGenerator.GenerateSprite(args.file, node, generateType, spriteOptions);
                        
                        if (sprite != null)
                        {
                            sprite.name = nodeId;
                            args.importer.generatedAssets.Add(nodeId, sprite);
                            args.importer.generatedAssets.Add(nodeId, sprite.texture);
                            return sprite;
                        }
                    }
                    catch (SvgImportException e)
                    {
                        args.importer.LogError(e + $": {e.svg}", figmaNode);
                    }
                }

                return sprite;
            }

            return null;
        }
    }

    public class VectorNodeConverter : VectorNodeConverter<VectorNode, FigmaNode>
    {
    }
}