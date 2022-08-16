namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">The Figma file to be imported.</param>
        void ImportFile(FigmaFile file);
    }
}