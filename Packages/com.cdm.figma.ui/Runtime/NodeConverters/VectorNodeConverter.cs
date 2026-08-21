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

                ApplySolidTint(style, vectorNode, args);

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
        
        /// <summary>
        /// Applies the colour <see cref="NodeSpriteGenerator"/> leaves out of the texture when a
        /// shape is painted in a single solid colour. Call this wherever that generator is used,
        /// or such a shape renders white.
        /// </summary>
        /// <remarks>
        /// The colour is always enabled, even when there is no solid tint to hoist. A selectable's
        /// per-state styles only write the properties they enable, so a state that left the colour
        /// disabled would keep whatever tint the previous state applied and multiply it into this
        /// state's sprite, turning strokes and fills black on hover. Falling back to opaque white,
        /// which multiplies to a no-op, keeps every state resetting the colour to a known value
        /// while shapes that carry their own colours in the texture stay unchanged.
        /// </remarks>
        internal static void ApplySolidTint(ImageStyle style, SceneNode node, NodeConvertArgs args)
        {
            style.color.enabled = true;

            if (NodeSpriteGenerator.TryGetSolidTint(node, args.overrideNode as SceneNode, out var tint))
            {
                style.color.value = tint;
            }
            else
            {
                style.color.value = UnityEngine.Color.white;
            }
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
                        var generatedSprite = 
                            args.spriteGenerator.GetOrGenerateSprite(args.file, node, generateType, spriteOptions);
                        
                        if (generatedSprite.sprite != null)
                        {
                            if (generatedSprite.isNew)
                            {
                                generatedSprite.sprite.name = nodeId;
                                args.importer.generatedAssets.Add(nodeId, generatedSprite.sprite);
                                args.importer.generatedAssets.Add(nodeId, generatedSprite.sprite.texture);    
                            }
                            
                            return generatedSprite.sprite;
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