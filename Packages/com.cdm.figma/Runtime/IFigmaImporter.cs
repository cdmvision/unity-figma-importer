namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">The Figma file to be imported.</param>
        /// <param name="options">Importer options.</param>
        FigmaDesign ImportFile(FigmaFile file, Options options = null);
        
        public class Options
        {
            public string[] selectedPages { get; set; }
        }
    }
}