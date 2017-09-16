using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online.Store.Core.DTOs
{
    public class ProductComponentDTO
    {
        public ProductComponentDTO()
        {
            this.Medias = new List<ProductMediaDTO>();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName="componenttype")]
        public string ComponentType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string ComponentTitle { get; set; }

        [JsonProperty(PropertyName="detail")]
        public string ComponentDetail { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? UpdatedDate { get; set; }

        [JsonProperty(PropertyName = "medias")]
        public List<ProductMediaDTO> Medias { get; set; }
    }
}
