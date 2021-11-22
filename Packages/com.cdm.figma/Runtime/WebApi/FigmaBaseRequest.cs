namespace Cdm.Figma
{
    public abstract class FigmaBaseRequest
    {
        public string fileId { get; }
        public string personalAccessToken { get; }

        public FigmaBaseRequest(string personalAccessToken, string fileId)
        {
            this.personalAccessToken = personalAccessToken;
            this.fileId = fileId;
        }
    }
}