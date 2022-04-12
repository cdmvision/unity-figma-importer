namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        protected override NodeObject Convert(NodeObject parentObject, InstanceNode node, NodeConvertArgs args)
        {
            // TODO: Implement proper way.
            return new GroupNodeConverter().Convert(parentObject, (InstanceNode) node, args);
        }
    }
}