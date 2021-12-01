using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(GroupNodeConverter), menuName = AssetMenuRoot + "Group", order = AssetMenuOrder)]
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return XmlFactory.NewElement<VisualElement>(node, args);
        }
    }
}