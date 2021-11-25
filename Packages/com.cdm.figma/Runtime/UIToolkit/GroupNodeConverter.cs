using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(GroupNodeConverter), menuName = AssetMenuRoot + "Group", order = 20)]
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override XElement Convert(FigmaImporter importer, FigmaFile file, Node node)
        {
            var groupNode = (GroupNode) node;
            var groupXml = new XElement("VisualElement");
            groupXml.SetAttributeValue("figmaId", node.id);
            groupXml.SetAttributeValue("figmaName", node.name);

            var children = groupNode.children;

            foreach (var child in children)
            {
                if (importer.TryConvertNode(file, child, out var childElement))
                {
                    groupXml.Add(childElement);
                }
            }

            return groupXml;
        }
    }
}