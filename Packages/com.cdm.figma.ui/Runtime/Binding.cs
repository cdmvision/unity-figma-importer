using System;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [Serializable]
    public class Binding
    {
        public const string PathSeparator = "/";
        
        [SerializeField]
        private string _key;

        public string key => _key;

        [SerializeField]
        private string _path;

        public string path => path;
        
        [SerializeField]
        private FigmaNode _node;

        public FigmaNode node => _node;

        public Binding(string key, string path, FigmaNode node)
        {
            _key = key;
            _path = path;
            _node = node;
        }
    }
}