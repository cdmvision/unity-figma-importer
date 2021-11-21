namespace Cdm.Figma
{
    public abstract class FigmaBaseRequest
    {
        public string fileId { get; private set; }
        public string personalAccessToken { get; private set; }

        public FigmaBaseRequest(string personalAccessToken, string fileId)
        {
            this.personalAccessToken = personalAccessToken;
            this.fileId = fileId;
        }
    }
}