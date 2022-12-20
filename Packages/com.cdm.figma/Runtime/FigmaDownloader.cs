using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

        public async Task<FigmaFile> DownloadFileAsync(string fileId, string personalAccessToken, 
            IProgress<FigmaDownloaderProgress> progress = default, CancellationToken cancellationToken = default)
        {
            try
            {
                using (_figmaApi = new FigmaApi(personalAccessToken))
                {
                    _downloadedFiles = new Dictionary<string, FigmaFile>();
                    return await DownloadFileAsyncInternal(
                        fileId, personalAccessToken, false, progress, cancellationToken);
                }
            }
            finally
            {
                _figmaApi = null;
                _downloadedFiles = null;
            }
        }

        private async Task<FigmaFile> DownloadFileAsyncInternal(string fileId, string personalAccessToken,
            bool isDependency, IProgress<FigmaDownloaderProgress> progress, CancellationToken cancellationToken)
        {
            progress?.Report(new FigmaDownloaderProgress(fileId, 0f, isDependency));

            var fileContentJson = await _figmaApi.GetFileAsync(
                new FileRequest(fileId)
                {
                    geometry = "paths",
                    plugins = new[] { PluginData.Id }
                }, cancellationToken);

            var file = FigmaFile.Parse(fileContentJson);
            file.fileId = fileId;

            if (!isDependency)
            {
                if (!string.IsNullOrEmpty(file.thumbnailUrl))
                {
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
                progress?.Report(new FigmaDownloaderProgress(fileId, 0.5f, isDependency));
                
                file.fileDependencies = 
                    await DownloadFileDependenciesAsync(file, personalAccessToken, progress, cancellationToken);
            }
            
            progress?.Report(new FigmaDownloaderProgress(fileId, 1f, isDependency));

            return file;
        }

        private async Task<FigmaFileDependency[]> DownloadFileDependenciesAsync(
            FigmaFile mainFile, string personalAccessToken, IProgress<FigmaDownloaderProgress> progress, 
            CancellationToken cancellationToken)
        {
            // Find external components.
            var missingComponents = new Dictionary<string, List<string>>();
            FindMissingComponents(mainFile, missingComponents);

            var fileDependencies = new Dictionary<string, FigmaFileDependency>();

            foreach (var missingComponent in missingComponents)
            {
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
                                componentMetadata.fileKey, personalAccessToken, true, progress, cancellationToken);
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