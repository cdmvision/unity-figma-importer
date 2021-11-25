using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public interface INodeConverter
    {
        bool CanConvert(FigmaImporter importer, FigmaFile file, Node node);
        XElement Convert(FigmaImporter importer, FigmaFile file, Node node);
    }
}