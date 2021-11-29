using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(InstanceNodeConverter), 
        menuName = AssetMenuRoot + "Instance", order = AssetMenuOrder)]
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return FrameNodeConverter.Convert((InstanceNode) node, args);
        }
    }
}