using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">Figma file to be imported.</param>
        /// <param name="options">Optional import options.</param>
        Task ImportFileAsync(FigmaFile file, FigmaImportOptions options = null);
    }
    
    public class FigmaImportOptions
    {
        /// <summary>
        /// Set page IDs to import only selected pages. Leave as <c>null</c> to import all pages.
        /// </summary>
        public string[] pages;
    }
}