using Newtonsoft.Json;
using System;

namespace Online.Store.Core.DTOs
{
    public class PostDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "mediaDescription")]
        public string MediaDescription { get; set; }
        [JsonProperty(PropertyName = "mediaUrl")]
        public string MediaUrl { get; set; }
        [JsonProperty(PropertyName = "mediaType")]
        public string MediaType { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
