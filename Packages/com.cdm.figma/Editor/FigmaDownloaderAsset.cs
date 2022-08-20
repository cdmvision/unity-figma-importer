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
        public string assetExtension => _assetExtension;

        [SerializeField]
        private string _assetPath = "Resources/Figma/Files";

        /// <summary>
        /// The directory where downloaded Figma files are stored in.
        /// </summary>
        public string assetPath => _assetPath;
        
        [SerializeField]
        private List<string> _files = new List<string>();

        /// <summary>
        /// Figma file IDs to be downloaded.
        /// </summary>
        public List<string> files => _files;

        public virtual IFigmaDownloader GetDownloader()
        {
            return new FigmaDownloader();
        }
    }
}