using System.Linq;
using Cdm.Figma.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var nodeObject = GroupNodeConverter.Convert(parentObject, frameNode, args);
            
            if (frameNode.fills.Any() || frameNode.strokes.Any())
            {
                var options = new VectorImageUtils.SpriteOptions()
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    sampleCount = 8,
                    textureSize = 1024
                };

                var sprite = VectorImageUtils.CreateSpriteFromRect(frameNode, options);
                
                var image = nodeObject.gameObject.AddComponent<Image>();
                image.sprite = sprite;
                image.type = Image.Type.Sliced;
                image.color = new UnityEngine.Color(1f, 1f, 1f, frameNode.opacity);
            }
            
            return nodeObject;
        }
    }
}