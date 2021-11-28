using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(VectorNodeConverter), menuName = AssetMenuRoot + "Vector", order = 20)]
    public class VectorNodeConverter : NodeConverter<VectorNode>
    {
        public static NodeObject Convert(VectorNode node, NodeConvertArgs args)
        {
            var nodeObject =  NodeObject.NewNodeObject(node, args);

            var absoluteBoundingBox = node.absoluteBoundingBox;
            var width = absoluteBoundingBox.width;
            var height = absoluteBoundingBox.height;

            // Override width and height if size property is exist.
            if (node.size != null)
            {
                width = node.size.x;
                height = node.size.y;
            }

            
            
            return nodeObject;
        }
        
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return Convert((VectorNode) node, args);
        }
    }
}