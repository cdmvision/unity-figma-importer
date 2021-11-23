using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaImporter
    {
        Task ImportFileAsync(FigmaFile file);
    }
}