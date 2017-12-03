using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online.Store.Core.DTOs
{
    public class Product
    {
        public Product()
        {
            this.Components = new List<ProductComponent>();
        }

        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "sku")]
        public string SKU { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName="image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public decimal Rating { get; set; }

        [JsonProperty(PropertyName = "rates")]
        public int Rates { get; set; }

        [JsonProperty(PropertyName="created")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? UpdatedDate { get; set; }

        [JsonProperty(PropertyName="components")]
        public List<ProductComponent> Components { get; set; }
    }
}
