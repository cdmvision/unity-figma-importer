using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Cdm.Figma
{
    public class FigmaApi
    {
        private const string BaseUri = "https://api.figma.com/v1";
        
        /// <summary>
        /// Returns the document referred to by :key as a JSON object string. The file key can be parsed from any
        /// Figma file url: https://www.figma.com/file/:key/:title
        ///
        /// The components key contains a mapping from node IDs to component metadata. This is to help you
        /// determine which components each instance comes from.
        /// </summary>
        public static async Task<string> GetFileAsTextAsync(FigmaFileRequest fileRequest)
        {
            if (string.IsNullOrEmpty(fileRequest.personalAccessToken))
                throw new ArgumentException("Personal access token cannot be empty.");

            if (string.IsNullOrEmpty(fileRequest.fileId))
                throw new ArgumentException("File ID cannot be empty.");

            var uri = GetFileRequestUrl(fileRequest);
            
#if UNITY_EDITOR
            Debug.Log(uri);
#endif            
            var result = await GetContentAsync(uri, fileRequest.personalAccessToken);
            return result;
        }

        /// <summary>
        /// Returns the document referred to by :key as a <see cref="FigmaFile"/>.
        /// </summary>
        /// <seealso cref="GetFileAsTextAsync"/>
        public static async Task<FigmaFile> GetFileAsync(FigmaFileRequest fileRequest)
        {
            var result = await GetFileAsTextAsync(fileRequest);
            return FigmaFile.Parse(result);
        }
        
        /// <summary>
        /// Renders images from a file.
        /// 
        /// If no error occurs, "images" will be populated with a map from node IDs to URLs of the rendered images,
        /// and "status" will be omitted. The image assets will expire after 30 days.
        /// 
        /// Important: the image map may contain values that are null. This indicates that rendering of that specific
        /// node has failed. This may be due to the node id not existing, or other reasons such has the node having no
        /// renderable components. It is guaranteed that any node that was requested for rendering will be represented
        /// in this map whether or not the render succeeded.
        /// </summary>
        public static async Task<Dictionary<string, byte[]>> GetImageAsync(FigmaImageRequest imageRequest)
        {
            if (string.IsNullOrEmpty(imageRequest.personalAccessToken))
                throw new ArgumentException("Personal access token cannot be empty.");

            if (string.IsNullOrEmpty(imageRequest.fileId))
                throw new ArgumentException("File ID cannot be empty.");
            
            if (imageRequest.ids == null || imageRequest.ids.Length == 0)
                throw new ArgumentException("Image ids array must has at least one item.");
            
            // Get image download URLs.
            var uri = GetImageRequestUrl(imageRequest);
            Debug.Log(uri);
            
            var result = await GetContentAsync(uri, imageRequest.personalAccessToken);

            var response = JsonConvert.DeserializeObject<FigmaImageResponse>(result);
            if (response == null)
                throw new Exception("Cannot get images. Response is null.");

            if (!string.IsNullOrEmpty(response.error))
                throw new Exception(response.error);

            // Download images data.
            var images = new Dictionary<string, byte[]>();

            foreach (var (imageId, imageUrl) in response.images)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageData = await GetBytesAsync(imageUrl);
                    if (imageData == null)
                        throw new Exception($"Image '{imageId}' data could not be get from: {imageUrl}");
                    
                    images.Add(imageId, imageData);
                }
                else
                {
                    images.Add(imageId, null);
                }
            }

            return images;
        }

        public static async Task<byte[]> GetThumbnailImageAsync(string thumbnailUrl)
        {
            return await GetBytesAsync(thumbnailUrl, "image/png");
        }
        
        private static string GetFileRequestUrl(FigmaFileRequest request)
        {
            var url = $"{BaseUri}/files/{request.fileId}";
            var firstArg = true;

            if (!string.IsNullOrEmpty(request.version))
            {
                url = $"{url}?version={request.version}";
                firstArg = false;
            }

            if (request.depth.HasValue)
            {
                url = $"{url}{(firstArg ? "?" : "&")}depth={request.depth.Value}";
                firstArg = false;
            }
            
            if (!string.IsNullOrEmpty(request.geometry))
            {
                url = $"{url}{(firstArg ? "?" : "&")}geometry={request.geometry}";
                firstArg = false;
            }

            if (request.plugins != null && request.plugins.Length > 0)
            {
                url = $"{url}{(firstArg ? "?" : "&")}plugin_data={string.Join(",", request.plugins)}";
                firstArg = false;
            }

            return url;
        }

        private static string GetImageRequestUrl(FigmaImageRequest request)
        {
            var url = $"{BaseUri}/images/{request.fileId}";
            url = $"{url}?ids={string.Join(",", request.ids)}";
            
            if (!string.IsNullOrEmpty(request.version))
            {
                url = $"{url}&version={request.version}";
            }
            
            if (!string.IsNullOrEmpty(request.format))
            {
                url = $"{url}&format={request.format}";
            }
            
            if (request.scale.HasValue)
            {
                url = $"{url}&scale={request.scale.Value}";
            }
            
            url = $"{url}&svg_include_id={request.svgIncludeId.ToString().ToLower()}";
            url = $"{url}&svg_simplify_stroke={request.svgSimplifyStroke.ToString().ToLower()}";
            url = $"{url}&use_absolute_bounds={request.useAbsoluteBounds.ToString().ToLower()}";
            return url;
        }
        
        private static async Task<byte[]> GetBytesAsync(string url, string contentType = "")
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            var responseStream = httpResponse.GetResponseStream();
            if (responseStream != null)
            {
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }

            throw new WebException("Response stream is null");
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
                return await streamReader.ReadToEndAsync();
            }

            throw new WebException("Response stream is null");
        }
    }
}