using Newtonsoft.Json;
using System;

namespace Online.Store.Core.DTOs
{
    public class ProductMedia
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? UpdatedDate { get; set; }
    }
}
