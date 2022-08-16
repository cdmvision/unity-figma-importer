using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaDownloader
    {
        Task<FigmaFile> DownloadFileAsync(string fileID, string personalAccessToken);
    }
}