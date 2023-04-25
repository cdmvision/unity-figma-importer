using System;
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
        public Node overrideNode { get; private set; }

        public Dictionary<string, ComponentNode> componentPropertyAssignments { get; } =
            new Dictionary<string, ComponentNode>();

        public Dictionary<string, string> textPropertyAssignments { get; }
            = new Dictionary<string, string>();

        public NodeConvertArgs(FigmaImporter importer, FigmaFile file)
        {
            this.importer = importer;
            this.file = file;
        }

        public IDisposable OverrideNode(Node node)
        {
            return new NodeOverride(this, node);
        }

        /// It is used to restore the <see cref="NodeConvertArgs.overrideNode"/> automatically.
        private class NodeOverride : IDisposable
        {
            public NodeConvertArgs args { get; }
            public Node previousNode { get; }

            public NodeOverride(NodeConvertArgs args, Node currentNode)
            {
                this.args = args;
                this.previousNode = args.overrideNode;
                args.overrideNode = currentNode;
            }

            public void Dispose()
            {
                args.overrideNode = previousNode;
            }
        }
    }
}