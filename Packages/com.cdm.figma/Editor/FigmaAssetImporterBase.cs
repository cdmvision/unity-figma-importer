using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Cdm.Figma.Editor
{
    public abstract class FigmaAssetImporterBase : ScriptedImporter
    {
        protected const string DefaultExtension = "figma";
        protected const int ImportQueueOffset = 9999;

        protected const string FigmaIconColorPath = EditorHelper.PackageFolderPath +
                                                    "/Editor Default Resources/FigmaIcon-Color.png";

        protected const string FigmaIconFlatPath = EditorHelper.PackageFolderPath +
                                                   "/Editor Default Resources/FigmaIcon-Flat.png";

        [SerializeField]
        private FigmaFilePage[] _pages;

        /// <summary>
        /// Selected pages to be imported.
        /// </summary>
        public FigmaFilePage[] pages
        {
            get => _pages;
            set => _pages = value;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
#if PROFILE_FIGMA_IMPORT
            var logFile = Profiler.logFile;
            var enabled = Profiler.enabled;
            
            try
            {
                var assetName = Path.GetFileName(assetPath);
                var s = $"Profiler/{assetName}";
                Directory.CreateDirectory("Profiler");
                
                Profiler.logFile = s;
                Profiler.enableBinaryLog = true;
                Profiler.enabled = true;
                Profiler.BeginSample($"Import {assetName}");
#endif
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    ImportAsset(ctx);
                }
                finally
                {
                    // Also when the import throws: a bar left up blocks the editor.
                    EditorUtility.ClearProgressBar();
                }

                stopwatch.Stop();

                Debug.Log($"Importing '{ctx.assetPath}' took {stopwatch.ElapsedMilliseconds} ms.");
                
#if PROFILE_FIGMA_IMPORT
            }
            finally
            {
                
                Profiler.EndSample();
                Profiler.enabled = enabled;
                Profiler.logFile = logFile;
            }
#endif
        }

        /// <summary>
        /// Shows where the import has got to. An import blocks the main thread, so this call is
        /// also what forces the repaint.
        /// </summary>
        /// <remarks>
        /// Keep this to once per phase and once per page. A repaint is not free, and reporting per
        /// node or per sprite would show up in the import time.
        /// </remarks>
        private static void ReportProgress(string assetPath, string step, float progress)
        {
            EditorUtility.DisplayProgressBar($"Importing {Path.GetFileName(assetPath)}", step, progress);
        }

        private void ImportAsset(AssetImportContext ctx)
        {
            FigmaFile figmaFile;

            ReportProgress(ctx.assetPath, "Reading file", 0f);

            // Pages switched off in the settings. A list of what to drop, not what to keep, so a
            // page that is new in the file is parsed rather than imported empty. Null on a first
            // import, which is what populates the page list in the first place.
            var skipPageIds = importSettingsMissing || _pages == null
                ? null
                : new HashSet<string>(_pages.Where(p => !p.enabled).Select(p => p.id));

            using (var compressedStream = File.Open(ctx.assetPath, FileMode.Open))
            {
                figmaFile = FigmaFile.ParseBinary(compressedStream, skipPageIds);
            }
            
            UpdatePages(figmaFile);

            ReportProgress(ctx.assetPath, "Preparing converters", 0.2f);

            var figmaImporter = GetFigmaImporter(ctx);
            OnAssetImporting(ctx, figmaImporter, figmaFile);

            // Page conversion is the long part, so it drives 0.25 to 0.9 of the bar.
            var figmaDesign = figmaImporter.ImportFile(figmaFile, new IFigmaImporter.Options()
            {
                selectedPages = _pages.Where(p => p.enabled).Select(p => p.id).ToArray(),
                onPageProgress = (pageName, fraction) =>
                    ReportProgress(ctx.assetPath, $"Converting {pageName}", 0.25f + fraction * 0.65f)
            });

            ReportProgress(ctx.assetPath, "Finalizing", 0.95f);

            OnAssetImported(ctx, figmaImporter, figmaFile, figmaDesign);

            if (figmaDesign.thumbnail != null)
            {
                ctx.AddObjectToAsset("FigmaDesignPreview", figmaDesign.thumbnail);
            }

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(FigmaIconColorPath);
            ctx.AddObjectToAsset("FigmaDesign", figmaDesign.gameObject, icon);
            ctx.SetMainObject(figmaDesign.gameObject);
        }

        private void UpdatePages(FigmaFile file)
        {
            if (importSettingsMissing)
            {
                _pages = null;
            }

            var newPages = file.document.children ?? Array.Empty<PageNode>();
            newPages = newPages.Where(x => !x.IsIgnored()).ToArray();

            var oldPages = _pages;
            _pages = new FigmaFilePage[newPages.Length];

            for (var i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new FigmaFilePage(newPages[i].id, newPages[i].name);

                // Restore previously page status.
                if (oldPages != null)
                {
                    var oldPageIndex = Array.FindIndex(oldPages, x => x.id == _pages[i].id);
                    if (oldPageIndex >= 0)
                    {
                        _pages[i].enabled = oldPages[oldPageIndex].enabled;
                    }
                }
            }
        }

        protected virtual void OnAssetImporting(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile)
        {
        }

        protected virtual void OnAssetImported(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile, FigmaDesign figmaDesign)
        {
        }

        protected virtual string GetAssetExtension()
        {
            return DefaultExtension;
        }

        protected abstract IFigmaImporter GetFigmaImporter(AssetImportContext ctx);
    }

    [Serializable]
    public struct FigmaFilePage
    {
        public bool enabled;
        public string id;
        public string name;

        public FigmaFilePage(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.enabled = true;
        }
    }
}