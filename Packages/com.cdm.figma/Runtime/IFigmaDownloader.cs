using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cdm.Figma
{
    public interface IFigmaDownloader
    {
        Task<FigmaFile> DownloadFileAsync(string personalAccessToken, string fileId, string fileVersion = "", 
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

        /// <summary>
        /// Bytes received so far, or 0 when that is not being tracked. Separate from
        /// <see cref="progress"/>, which has no content length to work from on a chunked response.
        /// </summary>
        public long bytesDownloaded { get; }

        /// <summary>
        /// What is happening right now, for display. Empty when not reported.
        /// </summary>
        public string stage { get; }

        public FigmaDownloaderProgress(string fileId, float progress, bool isDependency)
            : this(fileId, progress, isDependency, 0, "")
        {
        }

        public FigmaDownloaderProgress(string fileId, float progress, bool isDependency,
            long bytesDownloaded, string stage = "")
        {
            this.fileId = fileId;
            this.progress = progress;
            this.isDependency = isDependency;
            this.bytesDownloaded = bytesDownloaded;
            this.stage = stage;
        }
    }
}