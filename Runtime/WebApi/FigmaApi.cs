using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    public class FigmaApi : IDisposable
    {
        private const string BaseUri = "https://api.figma.com/v1";
        
        private readonly HttpClient _httpClient;

        public FigmaApi(string personalAccessToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-FIGMA-TOKEN", personalAccessToken);
            
            // Always accept json, so we can get detailed error message from backend.
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true
            };
        }
        
        /// <summary>
        /// Returns the document referred to by :key as a JSON object string. The file key can be parsed from any
        /// Figma file url: https://www.figma.com/file/:key/:title
        ///
        /// The components key contains a mapping from node IDs to component metadata. This is to help you
        /// determine which components each instance comes from.
        /// </summary>
        public async Task<string> GetFileAsync(FileRequest fileRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fileRequest.fileId))
                throw new ArgumentException("File ID cannot be empty.");

            var url = GetFileRequestUrl(fileRequest);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets metadata on a component by key.
        /// </summary>
        public async Task<ComponentMetadata> GetComponentMetadataAsync(ComponentMetadataRequest requestData, 
            CancellationToken cancellationToken = default)
        {
            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData));
            
            if (string.IsNullOrEmpty(requestData.key))
                throw new ArgumentNullException(nameof(requestData.key), "Component key cannot be empty.");

            var url = $"{BaseUri}/components/{requestData.key}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        
            var json = await response.Content.ReadAsStringAsync();
            var metadataResponse = 
                JsonConvert.DeserializeObject<ComponentMetadataResponse>(json, JsonSerializerHelper.Settings);
            return metadataResponse?.metadata;
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
        public async Task<Dictionary<string, byte[]>> GetImageAsync(ImageRequest imageRequest,
            CancellationToken cancellationToken = default)
        {
            if (imageRequest == null)
                throw new ArgumentNullException(nameof(imageRequest));
            
            if (string.IsNullOrEmpty(imageRequest.fileId))
                throw new ArgumentException("File ID cannot be empty.");
            
            if (imageRequest.ids == null || imageRequest.ids.Length == 0)
                throw new ArgumentException("Image ids array must has at least one item.");
            
            // Get image download URLs.
            var url = GetImageRequestUrl(imageRequest);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(json, JsonSerializerHelper.Settings);
            if (imageResponse == null)
                return null;
                
            // Download images data.
            var images = new Dictionary<string, byte[]>();
                
            foreach (var (imageId, imageUrl) in imageResponse.images)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageData = await GetBytesAsync(imageUrl, "application/octet-stream", cancellationToken);
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

        public async Task<byte[]> GetThumbnailImageAsync(string thumbnailUrl,
            CancellationToken cancellationToken = default)
        {
            return await GetBytesAsync(thumbnailUrl, "image/png", cancellationToken);
        }
        
        private static string GetFileRequestUrl(FileRequest request)
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

        private static string GetImageRequestUrl(ImageRequest request)
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
        
        private async Task<byte[]> GetBytesAsync(string url, string contentType, 
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }


        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}