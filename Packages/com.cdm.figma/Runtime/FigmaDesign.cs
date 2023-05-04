using System;
using UnityEngine;

namespace Cdm.Figma
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class FigmaDesign : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        public string id
        {
            get => _id;
            private set => _id = value;
        }

        [SerializeField]
        private string _title;

        public string title
        {
            get => _title;
            private set => _title = value;
        }

        [SerializeField]
        private string _version;

        public string version
        {
            get => _version;
            private set => _version = value;
        }

        [SerializeField]
        private string _lastModified;

        public string lastModified
        {
            get => _lastModified;
            private set => _lastModified = value;
        }

        [SerializeField]
        private Texture2D _thumbnail;

        public Texture2D thumbnail
        {
            get => _thumbnail;
            private set => _thumbnail = value;
        }
        
        public static T Create<T>(FigmaFile file) where T : FigmaDesign
        {
            var go = new GameObject(file.name);
            
            var figmaFile = go.AddComponent<T>();
            figmaFile.id = file.fileId;
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
                    
#if UNITY_EDITOR
                    figmaFile.hideFlags = HideFlags.NotEditable;
#endif
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