using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
    public interface IFigmaDownloader
    {
        Task<JObject> DownloadFileAsync(string fileID, string personalAccessToken);
    }
}