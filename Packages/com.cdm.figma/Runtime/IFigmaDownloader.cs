using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaDownloader
    {
        Task<FigmaFile> DownloadFileAsync(string fileId, string personalAccessToken, 
            IProgress<FigmaDownloaderProgress> progress = default, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The data that is used to report Figma file downloading progress.
    /// </summary>
    public readonly struct FigmaDownloaderProgress
    {
        /// <summary>
        /// The file ID that is downloading.
        /// </summary>
        public string fileId { get; }
        
        /// <summary>
        /// The file that is downloading currently whether is dependency or not.
        /// </summary>
        public bool isDependency { get; }
        
        /// <summary>
        /// File download progress between [0, 1].
        /// </summary>
        public float progress { get; }

        public FigmaDownloaderProgress(string fileId, float progress, bool isDependency)
        {
            this.fileId = fileId;
            this.progress = progress;
            this.isDependency = isDependency;
        }
    }
}