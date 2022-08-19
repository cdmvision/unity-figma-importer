using Cdm.Figma.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : VectorNodeConverter<RectangleNode, FigmaNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, RectangleNode node, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = true;
            
            if (!args.importer.generatedAssets.TryGet<Sprite>(node.id, out var sprite))
            {
                sprite =
                    NodeSpriteGenerator.GenerateSprite(node, SpriteGenerateType.Rectangle, args.importer.spriteOptions);
                if (sprite != null)
                {
                    args.importer.generatedAssets.Add(node.id, sprite);
                    args.importer.generatedAssets.Add(node.id, sprite.texture);
                }
            }

            convertArgs.sourceSprite = sprite;
            return Convert(parentObject, node, args, convertArgs);
        }
    }
}