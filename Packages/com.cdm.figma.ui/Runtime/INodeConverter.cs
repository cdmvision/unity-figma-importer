using System.Collections.Generic;

namespace Cdm.Figma.UI
{
    public interface INodeConverter
    {
        bool CanConvert(Node node, NodeConvertArgs args);
        NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args);
    }

    public class NodeConvertArgs
    {
        public FigmaImporter importer { get; }
        public FigmaFile file { get; }
        public FigmaFileContent fileContent { get; }
        public bool applyStyles { get; }
        public List<ComponentSetNode> componentSets { get; } = new List<ComponentSetNode>();

        public NodeConvertArgs(FigmaImporter importer, FigmaFile file, FigmaFileContent fileContent,
            bool applyStyles = true)
        {
            this.importer = importer;
            this.file = file;
            this.fileContent = fileContent;
            this.applyStyles = applyStyles;
        }
    }
}