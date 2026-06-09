using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A comment or reply left by a user
    /// </summary>
    [DataContract]
    public class Comment
    {
        /// <summary>
        /// Unique identifier for comment.
        /// </summary>
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }

        [DataMember(Name = "client_meta")]
        public CommentMetadata metadata { get; set; }

        /// <summary>
        /// The time at which the comment was left
        /// </summary>
        [DataMember(Name = "created_at")]
        public DateTimeOffset createdAt { get; set; }

        /// <summary>
        /// The file in which the comment lives
        /// </summary>
        [DataMember(Name = "file_key")]
        public string fileKey { get; set; }

        /// <summary>
        /// (MISSING IN DOCS)
        /// The content of the comment
        /// </summary>
        [DataMember(Name = "message")]
        public string message { get; set; }

        /// <summary>
        /// Only set for top level comments. The number displayed with the
        /// comment in the UI
        /// </summary>
        [DataMember(Name = "order_id")]
        public float orderId { get; set; }

        /// <summary>
        /// If present, the id of the comment to which this is the reply
        /// </summary>
        [DataMember(Name = "parent_id")]
        public string parentId { get; set; }

        /// <summary>
        /// If set, when the comment was resolved
        /// </summary>
        [DataMember(Name = "resolved_at")]
        public DateTimeOffset? resolvedAt { get; set; }

        /// <summary>
        /// The user who left the comment
        /// </summary>
        [DataMember(Name = "user")]
        public User user { get; set; }
    }
}