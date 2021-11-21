using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    public class FigmaApi
    {
        private const string BaseUri = "https://api.figma.com/v1";

        public static string GetFigmaFileUrl(string fileId, string version = null)
        {
            var url = $"{BaseUri}/files/{fileId}";

            if (!string.IsNullOrEmpty(version))
            {
                url = $"{url}?version={version}";
            }

            return url;
        }

        public static async Task<string> GetFileAsTextAsync(FigmaFileRequest fileRequest)
        {
            if (string.IsNullOrEmpty(fileRequest.personalAccessToken))
                throw new ArgumentException("Personal access token cannot be empty.");

            if (string.IsNullOrEmpty(fileRequest.fileId))
                throw new ArgumentException("File ID cannot be empty.");

            var uri = GetFigmaFileUrl(fileRequest.fileId, fileRequest.version);
            var result = await GetContentAsync(uri, fileRequest.personalAccessToken);
            return result;
        }

        public static async Task<FigmaFile> GetFileAsync(FigmaFileRequest fileRequest)
        {
            var result = await GetFileAsTextAsync(fileRequest);
            return FigmaFile.FromText(result);
        }

        private static async Task<string> GetContentAsync(string requestUri, string personalAccessToken)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers["x-figma-token"] = personalAccessToken;

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            var responseStream = httpResponse.GetResponseStream();
            if (responseStream != null)
            {
                using var streamReader = new StreamReader(responseStream);
                var result = await streamReader.ReadToEndAsync();
                var json = Newtonsoft.Json.Linq.JObject.Parse(result);
                return json.ToString();
            }

            throw new WebException("Response stream is null");
        }
    }
}