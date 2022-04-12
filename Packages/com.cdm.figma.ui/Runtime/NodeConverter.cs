using UnityEngine;

namespace Cdm.Figma.UI
{
    public abstract class NodeConverter : INodeConverter
    {
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args);

        /*public virtual Vector2 GetPosition(INodeTransform transform)
        {
            return transform.relativeTransform.GetPosition();
        }

        public virtual Vector2 GetSize(INodeTransform transform)
        {
            return transform.size;
        }

        public virtual Vector3 GetScale(INodeTransform transform)
        {
            var scale = transform.relativeTransform.GetScale();
            return new Vector3(scale.x, scale.y, 1);
        }

        public virtual Quaternion GetRotation(INodeTransform transform)
        {
            return transform.relativeTransform.GetRotation();
        }*/
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }

        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (TNodeType) node, args);
        }

        protected abstract NodeObject Convert(NodeObject parentObject, TNodeType node, NodeConvertArgs args);
    }
}