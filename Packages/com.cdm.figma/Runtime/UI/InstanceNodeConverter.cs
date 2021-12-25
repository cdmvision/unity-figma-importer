namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return FrameNodeConverter.Convert((InstanceNode) node, args);
        }
    }
}