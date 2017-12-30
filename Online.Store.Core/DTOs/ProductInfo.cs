using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Core.DTOs
{
    public class ProductInfo
    {
        [JsonProperty(PropertyName = "id")]
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsSearchable, IsFilterable, IsRetrievable(true)]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [IsSearchable, IsFilterable]
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "sku")]
        public string SKU { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "description")]
        [IsSearchable, IsRetrievable(true)]
        [Analyzer(AnalyzerName.AsString.FrLucene)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "rating")]
        [IsSortable]
        public double Rating { get; set; }

        [JsonProperty(PropertyName = "rates")]
        public int Rates { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? UpdatedDate { get; set; }
    }
}
