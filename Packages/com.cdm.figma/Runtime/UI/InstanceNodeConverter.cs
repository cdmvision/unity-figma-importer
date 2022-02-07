namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return GroupNodeConverter.Convert(parentObject, (InstanceNode) node, args);
        }
    }
}