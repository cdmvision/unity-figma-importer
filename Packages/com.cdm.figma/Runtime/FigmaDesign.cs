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
        
        private static bool _thumbnailDecodeWarned;

        public static T Create<T>(FigmaFile file) where T : FigmaDesign
        {
            // A constant, not file.name. Unity derives the local file ids of the components on this
            // object from its name, and a Figma branch carries its own document name, so switching
            // branches moved those ids and every reference stored elsewhere in the project turned
            // into None. The importer renames this object to the asset file name anyway, and the
            // document name is kept on title just below.
            var go = new GameObject(nameof(FigmaDesign));

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

                    var texture = new Texture2D(1, 1);
                    texture.name = "Thumbnail";

                    // LoadImage handles PNG and JPEG only. Figma serves WebP, so this fails and
                    // leaves an 8x8 placeholder behind. Better no thumbnail than a broken one.
                    if (texture.LoadImage(thumbnailData))
                    {
                        figmaFile.thumbnail = texture;
                    }
                    else
                    {
                        DestroyTexture(texture);

                        // Once per domain. With Figma serving WebP this is the usual outcome, and
                        // repeating it on every import would just be noise.
                        if (!_thumbnailDecodeWarned)
                        {
                            _thumbnailDecodeWarned = true;

                            Debug.LogWarning(
                                $"Thumbnail of '{file.name}' could not be decoded and has been skipped. " +
                                "Unity loads PNG and JPEG only, and Figma serves thumbnails as WebP.");
                        }
                    }

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

        /// <summary>
        /// Disposes of a texture that is not going to be used.
        /// </summary>
        private static void DestroyTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(texture);
                return;
            }
#endif
            UnityEngine.Object.Destroy(texture);
        }
    }
}