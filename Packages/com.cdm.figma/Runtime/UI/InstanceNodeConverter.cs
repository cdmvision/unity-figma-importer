using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(InstanceNodeConverter), menuName = AssetMenuRoot + "Instance", order = 20)]
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return FrameNodeConverter.Convert((InstanceNode) node, args);
        }
    }
}