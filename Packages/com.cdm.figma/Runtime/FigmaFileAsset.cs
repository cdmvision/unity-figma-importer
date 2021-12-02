using System;
using System.Collections.Generic;
using UnityEngine;
using Cdm.Figma.Utils;

namespace Cdm.Figma
{
    [Serializable]
    public class FigmaFilePage
    {
        public bool enabled = true;
        public string id;
        public string name;
    }
    
    public class FigmaFileAsset : ScriptableObject
    {
        [SerializeField]
        private string _id;

        public string id
        {
            get => _id;
            internal set => _id = value;
        }

        [SerializeField]
        private string _title;

        public string title
        {
            get => _title;
            internal set => _title = value;
        }

        [SerializeField]
        private string _version;

        public string version
        {
            get => _version;
            internal set => _version = value;
        }

        [SerializeField]
        private string _lastModified;

        public string lastModified
        {
            get => _lastModified;
            internal set => _lastModified = value;
        }

        [SerializeField]
        private Texture2D _thumbnail;

        public Texture2D thumbnail
        {
            get => _thumbnail;
            internal set => _thumbnail = value;
        }

        [SerializeField]
        private TextAsset _content;

        public TextAsset content
        {
            get => _content;
            internal set => _content = value;
        }

        [SerializeField]
        private FigmaFilePage[] _pages = new FigmaFilePage[0];
        
        public FigmaFilePage[] pages
        {
            get => _pages;
            internal set => _pages = value;
        }

        [SerializeField]
        private SerializableDictionary<string, UnityEngine.Object> _graphics = 
            new SerializableDictionary<string, UnityEngine.Object>();
        
        public IDictionary<string, UnityEngine.Object> graphics => _graphics;
        
        [SerializeField]
        private SerializableDictionary<FontDescriptor, UnityEngine.Object> _fonts = 
            new SerializableDictionary<FontDescriptor, UnityEngine.Object>();
        
        public IDictionary<FontDescriptor, UnityEngine.Object> fonts => _fonts;

        public FigmaFile GetFile()
        {
            return content == null ? null : FigmaFile.FromString(content.text);
        }
    }
}