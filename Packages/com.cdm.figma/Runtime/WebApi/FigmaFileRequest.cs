namespace Cdm.Figma
{
    public class FigmaFileRequest : FigmaBaseRequest
    {
        public string version { get; private set; }

        public FigmaFileRequest(string personalAccessToken, string fileId, string version = null)
            : base(personalAccessToken, fileId)
        {
            this.version = version;
        }
    }
}