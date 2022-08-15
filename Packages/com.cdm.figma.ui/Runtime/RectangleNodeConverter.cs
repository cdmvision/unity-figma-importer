using Cdm.Figma.Utils;

namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : VectorNodeConverter<RectangleNode, NodeObject>
    {
        protected override NodeObject Convert(NodeObject parentObject, RectangleNode node, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = true;

            if (!args.generatedSprites.TryGetValue(node.id, out var sprite))
            {
                sprite =
                    NodeSpriteGenerator.GenerateSprite(node, SpriteGenerateType.Rectangle, convertArgs.spriteOptions);
                if (sprite != null)
                {
                    args.generatedSprites.Add(node.id, sprite);
                }
            }

            convertArgs.sourceSprite = sprite;
            return Convert(parentObject, node, args, convertArgs);
        }
    }
}