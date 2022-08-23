using System.Threading;
using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaDownloader
    {
        Task<FigmaFile> DownloadFileAsync(string fileId, string personalAccessToken, 
            CancellationToken cancellationToken = default);
    }
}