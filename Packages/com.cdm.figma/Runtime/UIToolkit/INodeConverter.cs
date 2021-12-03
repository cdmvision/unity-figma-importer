using System.Collections.Generic;
using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public interface INodeConverter
    {
        bool CanConvert(Node node, NodeConvertArgs args);
        NodeElement Convert(Node node, NodeConvertArgs args);
    }

    public class NodeConvertArgs
    {
        public FigmaImporter importer { get; }
        public FigmaFile file { get; }
        public FigmaFileContent fileContent { get; }
        public XNamespaces namespaces { get; set; }
        public List<ComponentSetNode> componentSets { get; } = new List<ComponentSetNode>();
        
        public NodeConvertArgs(FigmaImporter importer, FigmaFile file, FigmaFileContent fileContent)
        {
            this.importer = importer;
            this.file = file;
            this.fileContent = fileContent;
        }
    }
    
    public class XNamespaces
    {
        public XNamespace engine;
        public XNamespace editor;

        public XNamespaces()
        {
        }

        public XNamespaces(XNamespace engine, XNamespace editor)
        {
            this.engine = engine;
            this.editor = editor;
        }
    }
}