using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The user who left the comment.
    ///
    /// A description of a user.
    /// </summary>
    [DataContract]
    public class User
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }
        
        [DataMember(Name = "handle")]
        public string handle { get; set; }

        [DataMember(Name = "img_url")]
        public string imageUrl { get; set; }
    }
}