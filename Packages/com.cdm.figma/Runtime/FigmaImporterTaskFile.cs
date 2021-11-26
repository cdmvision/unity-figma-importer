using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma
{
    [CreateAssetMenu(fileName = nameof(FigmaImporterTaskFile), 
        menuName = FigmaImporter.AssetMenuRoot + "Figma Importer Task File", order = 0)]
    public class FigmaImporterTaskFile : ScriptableObject
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

        [SerializeField]
        private FigmaImporter _importer;

        public FigmaImporter importer
        {
            get => _importer;
            set => _importer = value;
        }
    }
}