namespace Cdm.Figma.UIToolkit
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
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

        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return GroupNodeConverter.Convert(parentElement, (InstanceNode) node, args);
        }
    }
}