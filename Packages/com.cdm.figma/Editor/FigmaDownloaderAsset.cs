using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.Editor
{
    [CreateAssetMenu(fileName = "Figma Downloader", menuName = AssetMenuRoot + "Figma Downloader", order = 0)]
    public class FigmaDownloaderAsset : ScriptableObject
    {
        protected const string AssetMenuRoot = "Cdm/Figma/";

        [SerializeField]
        private string _personalAccessToken;

        /// <summary>
        /// A personal access token gives the holder access to a Figma account through the API to be able
        /// to download Figma files.
        /// </summary>
        public string personalAccessToken
        {
            get => _personalAccessToken;
            set => _personalAccessToken = value;
        }

        [SerializeField]
        private string _assetExtension = "figma";

        /// <summary>
        /// The extension of the downloaded Figma files. Appropriate Figma asset importer used regarding to the
        /// extension.
        /// </summary>
        public string assetExtension
        {
            get => _assetExtension;
            set => _assetExtension = value;
        }

        [SerializeField]
        private string _assetPath = "Resources/Figma/Files";

        /// <summary>
        /// The directory where downloaded Figma files are stored in.
        /// </summary>
        public string assetPath
        {
            get => _assetPath;
            set => _assetPath = value;
        }

        [SerializeField]
        private bool _downloadDependencies = true;

        /// <inheritdoc cref="FigmaDownloader.downloadDependencies"/>
        public bool downloadDependencies
        {
            get => _downloadDependencies;
            set => _downloadDependencies = value;
        }
        
        [SerializeField]
        private bool _downloadImages = true;

        /// <inheritdoc cref="FigmaDownloader.downloadImages"/>
        public bool downloadImages
        {
            get => _downloadImages;
            set => _downloadImages = value;
        }

        [SerializeField]
        private List<File> _files = new List<File>();

        /// <summary>
        /// Figma file IDs to be downloaded.
        /// </summary>
        public List<File> files => _files;

        public virtual FigmaDownloader GetDownloader()
        {
            return new FigmaDownloader()
            {
                downloadDependencies = downloadDependencies
            };
        }
        
        [Serializable]
        public class File
        {
            public string id;
            public string name;
            public string defaultName;

            public override string ToString()
            {
                return id;
            }
        }
    }
}