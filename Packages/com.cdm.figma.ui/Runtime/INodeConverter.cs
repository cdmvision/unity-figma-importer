using System.Collections.Generic;

namespace Cdm.Figma.UI
{
    public interface INodeConverter
    {
        bool CanConvert(Node node, NodeConvertArgs args);
        FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args);
    }

    public class NodeConvertArgs
    {
        public FigmaImporter importer { get; }
        public FigmaFile file { get; }

        public Dictionary<string, ComponentNode> componentPropertyAssignments { get; } =
            new Dictionary<string, ComponentNode>();

        public NodeConvertArgs(FigmaImporter importer, FigmaFile file)
        {
            this.importer = importer;
            this.file = file;
        }
    }
}