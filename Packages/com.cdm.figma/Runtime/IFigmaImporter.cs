using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        FigmaFile CreateFile(string fileId, string fileContent, byte[] thumbnailData = null);

        /// <summary>
        /// Imports pages and all their nodes from Figma file given into Unity.
        /// </summary>
        /// <param name="file">Figma file to be imported</param>
        Task ImportFileAsync(FigmaFile file);
    }
}