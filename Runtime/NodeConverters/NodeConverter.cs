using UnityEngine;

namespace Cdm.Figma.UI
{
    public abstract class NodeConverter : INodeConverter
    {
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }

        public override FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args)
        {
            var figmaNode = Convert(parentObject, (TNodeType)node, args);
            
            if (figmaNode != null && node is INodeTransform nodeTransform)
            {
                var rotationAngle = nodeTransform.GetRotationAngle();
                if (Mathf.Abs(rotationAngle) > 0.001f)
                {
                    args.importer.LogWarning(
                        $"Node import might be inaccurate due to the rotation ({rotationAngle}).", figmaNode);    
                }
            }
            
            return figmaNode;
        }
        
        protected abstract FigmaNode Convert(FigmaNode parentObject, TNodeType node, NodeConvertArgs args);
    }
}