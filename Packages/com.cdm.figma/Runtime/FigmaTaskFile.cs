using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma
{

    public abstract class FigmaTaskFile : ScriptableObject
    {
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
        private string _graphicsPath = "Resources/Figma/Graphics";

        public string graphicsPath => _graphicsPath;

        [SerializeField]
        private List<string> _files = new List<string>();

        public List<string> fileIds => _files;

        public abstract IFigmaImporter GetImporter();
    }

    public abstract class FigmaTaskFile<TImporter> : FigmaTaskFile where TImporter : FigmaImporter
    {
        [SerializeField]
        private TImporter _importer;

        public TImporter importer
        {
            get => _importer;
            set => _importer = value;
        }
        
        public override IFigmaImporter GetImporter() => importer;
    }
}