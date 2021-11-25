using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        Task ImportFileAsync(FigmaFile file, FigmaImportOptions options = null);
    }
    
    public class FigmaImportOptions
    {
        public string[] pages;
    }
}