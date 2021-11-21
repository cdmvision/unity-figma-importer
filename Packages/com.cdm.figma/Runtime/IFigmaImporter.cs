namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        RootNode GetDocument(FigmaFileAsset file);
    }
}