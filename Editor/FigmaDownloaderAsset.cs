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
        private string _fileId;

        public string fileId
        {
            get => _fileId;
            set => _fileId = value;
        }

        [SerializeField, Tooltip("Optional")]
        private string _fileVersion;
        
        public string fileVersion
        {
            get => _fileVersion;
            set => _fileVersion = value;
        }

        [SerializeField, Tooltip("Optional")]
        private string _fileName;
        
        public string fileName
        {
            get => _fileName;
            set => _fileName = value;
        }
        
        [SerializeField]
        private string _defaultFileName;
        
        public string defaultFileName
        {
            get => _defaultFileName;
            internal set => _defaultFileName = value;
        }

        public virtual FigmaDownloader GetDownloader()
        {
            return new FigmaDownloader()
            {
                downloadDependencies = downloadDependencies,
                downloadImages = downloadImages
            };
        }
    }
}