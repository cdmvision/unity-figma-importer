using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Cdm.Figma
{
    [Serializable]
    public class FigmaFilePage
    {
        public bool enabled = true;
        public string id;
        public string name;

        public FigmaFilePage()
        {
        }
        
        public FigmaFilePage(string id, string name)
        {
        }
        
        public FigmaFilePage(FigmaFilePage other)
        {
            enabled = other.enabled;
            id = other.id;
            name = other.name;
        } 
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
        private FigmaFilePage[] _pages = Array.Empty<FigmaFilePage>();

        public FigmaFilePage[] pages
        {
            get => _pages;
            set => _pages = value;
        }

        public FigmaFileContent GetFileContent()
        {
            return content == null ? null : FigmaFileContent.FromString(content.text);
        }

        public static T Create<T>(string fileID, string fileJson, byte[] thumbnailData = null)
            where T : FigmaFile
        {
            var fileContent = FigmaFileContent.FromString(fileJson);
            var figmaFile = CreateInstance<T>();
            figmaFile.id = fileID;
            figmaFile.title = fileContent.name;
            figmaFile.version = fileContent.version;
            figmaFile.lastModified = fileContent.lastModified.ToString("u");
            figmaFile.content = new TextAsset(JObject.Parse(fileJson).ToString(Newtonsoft.Json.Formatting.Indented));
            figmaFile.content.name = "File";
            
            if (thumbnailData != null)
            {
                figmaFile.thumbnail = new Texture2D(1, 1);
                figmaFile.thumbnail.name = "Thumbnail";
                figmaFile.thumbnail.LoadImage(thumbnailData);
            }
            
            var pages = fileContent.document.children;
            figmaFile.pages = new FigmaFilePage[pages.Length];

            for (var i = 0; i < pages.Length; i++)
            {
                figmaFile.pages[i] = new FigmaFilePage()
                {
                    enabled = true,
                    id = pages[i].id,
                    name = pages[i].name
                };
            }
            
            return figmaFile;
        }

        public void CopyTo(FigmaFile other)
        {
            MergeTo(other, true);
        }

        public virtual void MergeTo(FigmaFile other, bool overwrite = false)
        {
            var oldPages = other.pages;

            if (other.content != null)
            {
                DestroyImmediate(other.content, true);
                other.content = null;
            }
            
            if (other.thumbnail != null)
            {
                DestroyImmediate(other.thumbnail, true);
                other.thumbnail = null;
            }

            other.id = _id;
            other.title = title;
            other.version = version;
            other.lastModified = lastModified;
            other.content = content;
            other.thumbnail = thumbnail;
            other.pages = new FigmaFilePage[pages.Length];

            if (!overwrite)
            {
                // Preserve old page selection.
                for (var i = 0; i < other.pages.Length; i++)
                {
                    other.pages[i] = new FigmaFilePage(pages[i]);
                    
                    var pageIndex = Array.FindIndex(oldPages, x => x.id == other.pages[i].id);
                    if (pageIndex >= 0)
                    {
                        other.pages[i].enabled = oldPages[pageIndex].enabled;
                    }
                }
            }
        }
    }
}