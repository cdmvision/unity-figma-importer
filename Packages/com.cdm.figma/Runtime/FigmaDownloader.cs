using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma
{
    public class FigmaDownloader : IFigmaDownloader
    {
        private Dictionary<string, FigmaFile> _downloadedFiles;
        
        /// <summary>
        /// If set <c>true</c>, dependent components shared from external files are downloaded as well.
        /// </summary>
        public bool downloadDependencies { get; set; } = true;

        public async Task<FigmaFile> DownloadFileAsync(string fileID, string personalAccessToken)
        {
            try
            {
                _downloadedFiles = new Dictionary<string, FigmaFile>();
                return await DownloadFileAsyncInternal(fileID, personalAccessToken);
            }
            finally
            {
                _downloadedFiles = null;
            }
        }

        private async Task<FigmaFile> DownloadFileAsyncInternal(string fileID, string personalAccessToken)
        {
            Debug.Log($"Downloading file: {fileID}");

            var fileContentJson = await FigmaApi.GetFileAsTextAsync(
                new FileRequest(personalAccessToken, fileID)
                {
                    geometry = "paths",
                    plugins = new[] { PluginData.Id }
                });

            var file = FigmaFile.Parse(fileContentJson);
            file.fileID = fileID;

            if (!string.IsNullOrEmpty(file.thumbnailUrl))
            {
                var thumbnail = await FigmaApi.GetThumbnailImageAsync(file.thumbnailUrl);
                if (thumbnail != null)
                {
                    file.thumbnail = Convert.ToBase64String(thumbnail);
                }
            }

            _downloadedFiles.Add(file.fileID, file);

            file.BuildHierarchy();
            
            if (downloadDependencies)
            {
                file.fileDependencies = await DownloadFileDependenciesAsync(file, personalAccessToken);    
            }
            
            return file;
        }

        private async Task<FigmaFileDependency[]> DownloadFileDependenciesAsync(
            FigmaFile mainFile, string personalAccessToken)
        {
            // Find external components.
            var missingComponents = new HashSet<string>();
            FindMissingComponents(mainFile, missingComponents);

            var fileDependencies = new Dictionary<string, FigmaFileDependency>();

            foreach (var componentKey in missingComponents)
            {
                var componentMetadata =
                    await FigmaApi.GetComponentMetadataAsync(
                        new ComponentMetadataRequest(personalAccessToken, componentKey));

                if (componentMetadata != null)
                {
                    // Download file containing the component if does not exist.
                    if (!_downloadedFiles.ContainsKey(componentMetadata.fileKey))
                    {
                        await DownloadFileAsyncInternal(componentMetadata.fileKey, personalAccessToken);
                    }

                    {
                        var file = _downloadedFiles[componentMetadata.fileKey];

                        if (file.components.TryGetValue(componentMetadata.nodeId, out var component) &&
                            file.componentNodes.TryGetValue(componentMetadata.nodeId, out var componentNode))
                        {
                            FigmaFileDependency fileDependency;
                            if (!fileDependencies.ContainsKey(file.fileID))
                            {
                                fileDependency = new FigmaFileDependency();
                                fileDependency.fileID = file.fileID;
                                fileDependencies.Add(fileDependency.fileID, fileDependency);
                            }
                            else
                            {
                                fileDependency = fileDependencies[file.fileID];    
                            }
                            
                            fileDependency.components.Add(componentMetadata.nodeId, component);
                            fileDependency.componentNodes.Add(componentMetadata.nodeId, componentNode);

                            // Check component set.
                            if (!string.IsNullOrEmpty(component.componentSetId))
                            {
                                if (file.componentSets.TryGetValue(component.componentSetId, out var componentSet) &&
                                    file.componentSetNodes.TryGetValue(component.componentSetId,
                                        out var componentSetNode))
                                {
                                    fileDependency.componentSets.Add(component.componentSetId, componentSet);
                                    fileDependency.componentSetNodes.Add(component.componentSetId, componentSetNode);
                                }
                                else
                                {
                                    Debug.LogWarning(
                                        $"Component set node '{component.componentSetId}' could not be found in file '{file.fileID}'");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Component node '{componentMetadata.nodeId}' could not be found in file '{file.fileID}'");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Component metadata with key '{componentKey}' could not be get.");
                }
            }

            return fileDependencies.Values.ToArray();
        }

        private static void FindMissingComponents(FigmaFile figmaFile, ISet<string> components)
        {
            figmaFile.document.Traverse(node =>
            {
                var instanceNode = (InstanceNode)node;

                if (!string.IsNullOrEmpty(instanceNode.componentId))
                {
                    if (!figmaFile.componentNodes.ContainsKey(instanceNode.componentId))
                    {
                        if (figmaFile.components.TryGetValue(instanceNode.componentId, out var component))
                        {
                            components.Add(component.key);
                        }
                    }
                }

                return true;
            }, NodeType.Instance);
        }
    }
}