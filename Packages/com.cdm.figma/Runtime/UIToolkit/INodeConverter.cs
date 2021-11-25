using System.Xml;

namespace Cdm.Figma.UIToolkit
{
    public interface INodeConverter
    {
        bool CanConvert(FigmaImporter importer, FigmaFile file, Node node);
        XmlElement Convert(FigmaImporter importer, FigmaFile file, Node node);
    }
}