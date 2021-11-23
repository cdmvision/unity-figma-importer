using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public interface IComponentConverter
    {
        bool CanConvert(UIToolkitFigmaImporter importer, FigmaFile file, Node node);
        VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node);
    }
}