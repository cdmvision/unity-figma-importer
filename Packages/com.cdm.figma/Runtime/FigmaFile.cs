using System;
using System.Collections.Generic;
using UnityEngine;
using Cdm.Figma.Utils;
using Object = UnityEngine.Object;

namespace Cdm.Figma
{
    [Serializable]
    public class FigmaFilePage
    {
        public bool enabled = true;
        public string id;
        public string name;
    }
    
    public class FigmaFile : ScriptableObject
    {
        [SerializeField]
        private string _id;

        public string id
        {
            get => _id;
            set => _id = value;
        }

        [SerializeField]
        private string _title;

        public string title
        {
            get => _title;
            set => _title = value;
        }

        [SerializeField]
        private string _version;

        public string version
        {
            get => _version;
            set => _version = value;
        }

        [SerializeField]
        private string _lastModified;

        public string lastModified
        {
            get => _lastModified;
            set => _lastModified = value;
        }

        [SerializeField]
        private Texture2D _thumbnail;

        public Texture2D thumbnail
        {
            get => _thumbnail;
            set => _thumbnail = value;
        }

        [SerializeField]
        private TextAsset _content;

        public TextAsset content
        {
            get => _content;
            set => _content = value;
        }

        [SerializeField]
        private FigmaFilePage[] _pages = new FigmaFilePage[0];
        
        public FigmaFilePage[] pages
        {
            get => _pages;
            set => _pages = value;
        }
        
        public FigmaFileContent GetFileContent()
        {
            return content == null ? null : FigmaFileContent.FromString(content.text);
        }
    }
}