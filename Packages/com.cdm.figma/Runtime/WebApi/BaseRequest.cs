namespace Cdm.Figma
{
    public abstract class BaseRequest
    {
        public string personalAccessToken { get; }

        public BaseRequest(string personalAccessToken)
        {
            this.personalAccessToken = personalAccessToken;
        }
    }
}