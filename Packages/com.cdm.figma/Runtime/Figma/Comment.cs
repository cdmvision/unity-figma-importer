using System;
using Newtonsoft.Json;

namespace Cdm.Figma
{
/// <summary>
    /// A comment or reply left by a user
    /// </summary>
    [Serializable]
    public partial class Comment
    {
        /// <summary>
        /// Unique identifier for comment.
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public string id { get; set; }
        
        [JsonProperty("client_meta")]
        public CommentMetadata metadata { get; set; }

        /// <summary>
        /// The time at which the comment was left
        /// </summary>
        [JsonProperty("created_at")]
        public DateTimeOffset createdAt { get; set; }

        /// <summary>
        /// The file in which the comment lives
        /// </summary>
        [JsonProperty("file_key")]
        public string fileKey { get; set; }
        
        /// <summary>
        /// (MISSING IN DOCS)
        /// The content of the comment
        /// </summary>
        [JsonProperty("message")]
        public string message { get; set; }

        /// <summary>
        /// Only set for top level comments. The number displayed with the
        /// comment in the UI
        /// </summary>
        [JsonProperty("order_id")]
        public float orderId { get; set; }

        /// <summary>
        /// If present, the id of the comment to which this is the reply
        /// </summary>
        [JsonProperty("parent_id")]
        public string parentId { get; set; }

        /// <summary>
        /// If set, when the comment was resolved
        /// </summary>
        [JsonProperty("resolved_at")]
        public DateTimeOffset? resolvedAt { get; set; }

        /// <summary>
        /// The user who left the comment
        /// </summary>
        [JsonProperty("user")]
        public User user { get; set; }
    }
}