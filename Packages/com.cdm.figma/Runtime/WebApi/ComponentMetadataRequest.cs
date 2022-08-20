namespace Cdm.Figma
{
    public class ComponentMetadataRequest : BaseRequest
    {
        /// <summary>
        /// Component key.
        /// </summary>
        public string key { get; }
        
        public ComponentMetadataRequest(string personalAccessToken, string key) : base(personalAccessToken)
        {
            this.key = key;
        }
    }
}