using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma
{
    [CreateAssetMenu(fileName = nameof(FigmaImporter), menuName = "Cdm/Figma/Figma Importer")]
    public class FigmaImporter : ScriptableObject
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
        private List<string> _files = new List<string>();

        public List<string> files => _files;
    }
}