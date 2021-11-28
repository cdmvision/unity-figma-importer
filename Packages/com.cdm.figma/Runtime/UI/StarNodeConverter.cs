using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(StarNodeConverter), menuName = AssetMenuRoot + "Star", order = 20)]
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((StarNode) node, args);
        }
    }
}