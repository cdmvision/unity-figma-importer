using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public interface INodeConverter
    {
        bool CanConvert(Node node, NodeConvertArgs args);
        XElement Convert(Node node, NodeConvertArgs args);
    }

    public class NodeConvertArgs
    {
        public FigmaImporter importer { get; }
        public FigmaFile file { get; }
        public XNamespaces namespaces { get; }

        public NodeConvertArgs(FigmaImporter importer, FigmaFile file, XNamespaces namespaces)
        {
            this.importer = importer;
            this.file = file;
            this.namespaces = namespaces;
        }
    }
    
    public class XNamespaces
    {
        public XNamespace engine;
        public XNamespace editor;
    }
}