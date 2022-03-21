using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma
{
    public abstract class FigmaTaskFile : ScriptableObject
    { 
        protected const string AssetMenuRoot = "Cdm/Figma/";
        
        [SerializeField]
        private string _personalAccessToken;

        public string personalAccessToken
        {
            get => _personalAccessToken;
            set => _personalAccessToken = value;
        }

        [SerializeField]
        private string _assetsPath = "Resources/Figma/Files";

        public string assetsPath => _assetsPath;
        
        [SerializeField]
        private string _documentsPath = "Resources/Figma/Documents";

        public string documentsPath => _documentsPath;
        
        [SerializeField]
        private string _scriptsPath = "Resources/Figma/Scripts";

        public string scriptsPath => _scriptsPath;
        
        [SerializeField]
        private string _graphicsPath = "Resources/Figma/Graphics";

        public string graphicsPath => _graphicsPath;

        [SerializeField]
        private bool _generateScripts = true;

        public bool generateScripts
        {
            get => _generateScripts;
            set => _generateScripts = value;
        }
        
        [SerializeField]
        private List<string> _files = new List<string>();

        public List<string> fileIds => _files;

        public abstract IFigmaImporter GetImporter();
        public abstract IFigmaDownloader GetDownloader();
    }

    public abstract class FigmaTaskFile<TImporter, TDownloader> : FigmaTaskFile 
        where TImporter : IFigmaImporter, new() where TDownloader : IFigmaDownloader, new() 
    {
        public TImporter importer { get; private set; } = new TImporter();
        public TDownloader downloader { get; private set; } = new TDownloader();
        
        public override IFigmaImporter GetImporter() => importer;
        public override IFigmaDownloader GetDownloader() => downloader;
    }
}