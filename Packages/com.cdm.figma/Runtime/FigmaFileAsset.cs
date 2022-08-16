using System;
using UnityEngine;

namespace Cdm.Figma
{
    public class FigmaFileAsset : ScriptableObject
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
        
        public static FigmaFileAsset Create(FigmaFile file)
        {
            var figmaFile = CreateInstance<FigmaFileAsset>();
            figmaFile.id = file.fileID;
            figmaFile.title = file.name;
            figmaFile.version = file.version;
            figmaFile.lastModified = file.lastModified.ToString("u");

            if (!string.IsNullOrEmpty(file.thumbnail))
            {
                try
                {
                    var thumbnailData = Convert.FromBase64String(file.thumbnail);
                    figmaFile.thumbnail = new Texture2D(1, 1);
                    figmaFile.thumbnail.name = "Thumbnail";
                    figmaFile.thumbnail.LoadImage(thumbnailData);
                }
                catch (Exception)
                {
                    Debug.LogWarning("Thumbnail image could not be loaded.");
                }
            }

            return figmaFile;
        }
    }
}