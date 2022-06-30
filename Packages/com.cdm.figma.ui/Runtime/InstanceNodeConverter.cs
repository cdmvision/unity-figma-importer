namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        /// <summary>
        /// Converts an instance node if <see cref="ComponentConverter"/>s fail.
        /// </summary>
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (!base.CanConvert(node, args))
                return false;
            
            var instanceNode = (InstanceNode) node;
            if (instanceNode.mainComponent == null)
                return false;

            if (instanceNode.mainComponent.componentSet != null)
                return false;

            return true;
        }
        
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode node, NodeConvertArgs args)
        {
            var frameNodeConverter = new FrameNodeConverter();
            return frameNodeConverter.Convert(parentObject, node, args);
        }
    }
}