using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Cdm.Figma
{
    public class FigmaDownloader : IFigmaDownloader
    {
        private Dictionary<string, FigmaFile> _downloadedFiles;
        private FigmaApi _figmaApi;

        /// <summary>
        /// If set <c>true</c>, dependent components shared from external files are downloaded as well.
        /// </summary>
        public bool downloadDependencies { get; set; } = true;

        /// <summary>
        /// If set <c>true</c>, all images present in image fills in a document are also downloaded.
        /// </summary>
        public bool downloadImages { get; set; } = true;

        public async Task<FigmaFile> DownloadFileAsync(
            string personalAccessToken, string fileId, string fileVersion = "",
            IProgress<FigmaDownloaderProgress> progress = default, CancellationToken cancellationToken = default)
        {
            try
            {
                using (_figmaApi = new FigmaApi(personalAccessToken))
                {
                    _downloadedFiles = new Dictionary<string, FigmaFile>();
                    return await DownloadFileAsyncInternal(
                        personalAccessToken, fileId, fileVersion, false, progress, 0f, 1f, cancellationToken);
                }
            }
            finally
            {
                _figmaApi = null;
                _downloadedFiles = null;
            }
        }

        /// <summary>
        /// Downloads a file's metadata: its name, version and branches, without the document.
        /// </summary>
        /// <remarks>
        /// Uses <c>depth=1</c>, which returns the pages and nothing below them. Branch data is
        /// unaffected by depth.
        /// </remarks>
        public virtual async Task<FigmaFile> DownloadFileMetadataAsync(
            string personalAccessToken, string fileId, string fileVersion = "",
            CancellationToken cancellationToken = default)
        {
            using var api = new FigmaApi(personalAccessToken);

            var fileContentJson = await api.GetFileAsync(
                new FileRequest(fileId)
                {
                    version = fileVersion,
                    depth = 1,
                    includeBranchData = true
                }, cancellationToken);

            var file = FigmaFile.Parse(fileContentJson);
            file.fileId = fileId;
            return file;
        }

        /// <summary>
        /// How far through the transfer to report, out of the 40% of a file's progress it covers.
        /// </summary>
        /// <remarks>
        /// Figma usually sends these chunked, with no content length to build a fraction from, so
        /// without a length this approaches the limit without reaching it: always moving, never
        /// claiming to be done.
        /// </remarks>
        private static float DownloadFraction(long bytesRead, long? totalBytes)
        {
            const float downloadShare = 0.4f;

            if (totalBytes.HasValue && totalBytes.Value > 0)
                return (float)(bytesRead / (double)totalBytes.Value) * downloadShare;

            // Half way to the limit at 16 MB, the rough order of these files.
            const double scale = 16.0 * 1024 * 1024;
            return (float)(downloadShare * (1.0 - scale / (scale + bytesRead)));
        }

        /// <param name="progressStart">Where this file's own 0 to 1 sits in the reported bar.</param>
        /// <param name="progressSpan">
        /// How much of the reported bar this file's own 0 to 1 covers.
        /// </param>
        /// <remarks>
        /// A dependency recurses into here, so without a range it would report from zero again and
        /// send the bar backwards.
        /// </remarks>
        private async Task<FigmaFile> DownloadFileAsyncInternal(
            string personalAccessToken, string fileId, string fileVersion,
            bool isDependency, IProgress<FigmaDownloaderProgress> progress,
            float progressStart, float progressSpan, CancellationToken cancellationToken)
        {
            void Report(float fraction, long bytesDownloaded, string stage)
            {
                progress?.Report(new FigmaDownloaderProgress(
                    fileId, progressStart + fraction * progressSpan, isDependency, bytesDownloaded, stage));
            }

            Report(0f, 0, "Connecting");

            // The transfer covers the first 40% of this file, the rest covers what follows it.
            var fileContentJson = await _figmaApi.GetFileAsync(
                new FileRequest(fileId)
                {
                    version = fileVersion,
                    geometry = "paths",
                    plugins = new[] { PluginData.Id },
                    includeBranchData = true
                }, cancellationToken,
                (bytesRead, totalBytes) => Report(DownloadFraction(bytesRead, totalBytes), bytesRead, "Downloading"));

            // Deserializing a large document takes seconds, so name the stage rather than
            // leaving the bar sitting at the end of the transfer.
            Report(0.45f, fileContentJson.Length, "Reading file");

            var file = FigmaFile.Parse(fileContentJson);
            file.fileId = fileId;

            if (!isDependency)
            {
                if (!string.IsNullOrEmpty(file.thumbnailUrl))
                {
                    Report(0.6f, 0, "Downloading thumbnail");

                    try
                    {
                        var thumbnail = await _figmaApi.GetThumbnailImageAsync(file.thumbnailUrl, cancellationToken);
                        if (thumbnail != null)
                        {
                            file.thumbnail = Convert.ToBase64String(thumbnail);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Debug.LogWarning($"File '{file.fileId}' thumbnail could not be downloaded.\n {e}");
                    }
                }

                if (downloadImages)
                {
                    // One request for every image fill, which is slow on a design with many.
                    Report(0.65f, 0, "Downloading images");

                    var images = await _figmaApi.GetImageFillsAsync(new ImageFillsRequest(fileId), cancellationToken);

                    foreach (var image in images)
                    {
                        file.images.Add(image.Key, Convert.ToBase64String(image.Value));
                    }
                }
            }

            _downloadedFiles.Add(file.fileId, file);

            file.BuildHierarchy();

            if (downloadDependencies)
            {
                Report(0.75f, 0, "Downloading dependencies");

                // Dependencies share the last quarter of this file's range.
                file.fileDependencies = await DownloadFileDependenciesAsync(
                    file, personalAccessToken, progress,
                    progressStart + 0.75f * progressSpan, 0.25f * progressSpan, cancellationToken);
            }

            Report(1f, 0, "");
            return file;
        }

        private async Task<FigmaFileDependency[]> DownloadFileDependenciesAsync(
            FigmaFile mainFile, string personalAccessToken, IProgress<FigmaDownloaderProgress> progress,
            float progressStart, float progressSpan, CancellationToken cancellationToken)
        {
            // Find external components.
            var missingComponents = new Dictionary<string, List<string>>();
            FindMissingComponents(mainFile, missingComponents);

            var fileDependencies = new Dictionary<string, FigmaFileDependency>();

            // An equal slice per lookup, so the bar moves forwards however many files this pulls.
            var componentSpan = missingComponents.Count > 0 ? progressSpan / missingComponents.Count : 0f;
            var componentIndex = 0;

            foreach (var missingComponent in missingComponents)
            {
                var componentStart = progressStart + componentIndex * componentSpan;
                componentIndex++;

                try
                {
                    var componentMetadata =
                        await _figmaApi.GetComponentMetadataAsync(
                            new ComponentMetadataRequest(missingComponent.Key), cancellationToken);

                    if (componentMetadata != null)
                    {
                        // Download file containing the component if does not exist.
                        if (!_downloadedFiles.ContainsKey(componentMetadata.fileKey))
                        {
                            await DownloadFileAsyncInternal(
                                personalAccessToken, componentMetadata.fileKey, "", true, progress,
                                componentStart, componentSpan, cancellationToken);
                        }

                        {
                            var file = _downloadedFiles[componentMetadata.fileKey];

                            if (file.components.TryGetValue(componentMetadata.nodeId, out var component) &&
                                file.componentNodes.TryGetValue(componentMetadata.nodeId, out var componentNode))
                            {
                                FigmaFileDependency fileDependency;
                                if (!fileDependencies.ContainsKey(file.fileId))
                                {
                                    fileDependency = new FigmaFileDependency();
                                    fileDependency.fileId = file.fileId;
                                    fileDependencies.Add(fileDependency.fileId, fileDependency);
                                }
                                else
                                {
                                    fileDependency = fileDependencies[file.fileId];
                                }

                                fileDependency.components.Add(componentMetadata.nodeId, component);
                                fileDependency.componentNodes.Add(componentMetadata.nodeId, componentNode);

                                // Check component set.
                                if (!string.IsNullOrEmpty(component.componentSetId))
                                {
                                    if (file.componentSets.TryGetValue(component.componentSetId,
                                            out var componentSet) &&
                                        file.componentSetNodes.TryGetValue(component.componentSetId,
                                            out var componentSetNode))
                                    {
                                        fileDependency.componentSets.Add(component.componentSetId, componentSet);
                                        fileDependency.componentSetNodes.Add(component.componentSetId,
                                            componentSetNode);
                                    }
                                    else
                                    {
                                        Debug.LogWarning(
                                            $"Component set node '{component.componentSetId}' could not be found in file '{file.fileId}'");
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogWarning(
                                    $"Component node '{componentMetadata.nodeId}' could not be found in file '{file.fileId}'");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Component metadata '{missingComponent.Key}' used by [{string.Join(", ", missingComponent.Value)}] does not exist.");
                    }
                }
                catch (HttpRequestException e)
                {
                    Debug.LogWarning(
                        $"Component metadata '{missingComponent.Key}' used by [{string.Join(", ", missingComponent.Value)}] could not be fetched: {e.Message}");
                }
            }

            return fileDependencies.Values.ToArray();
        }

        private static void FindMissingComponents(FigmaFile figmaFile, Dictionary<string, List<string>> components)
        {
            figmaFile.document.TraverseDfs(node =>
            {
                var instanceNode = (InstanceNode)node;

                if (!string.IsNullOrEmpty(instanceNode.componentId))
                {
                    if (!figmaFile.componentNodes.ContainsKey(instanceNode.componentId))
                    {
                        if (figmaFile.components.TryGetValue(instanceNode.componentId, out var component))
                        {
                            if (components.TryGetValue(component.key, out var nodes))
                            {
                                nodes.Add(instanceNode.id);
                            }
                            else
                            {
                                components.Add(component.key, new List<string>() { instanceNode.id });
                            }
                        }
                    }
                }

                return true;
            }, NodeType.Instance);
        }
    }
}