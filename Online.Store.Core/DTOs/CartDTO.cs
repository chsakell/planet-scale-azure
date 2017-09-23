using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online.Store.Core.DTOs
{
    public class CartDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<CartItemsDTO> Items { get; set; }

        [JsonProperty(PropertyName = "createddate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updateddate")]
        public DateTime? UpdateDate { get; set; }

        public CartDTO()
        {
            this.Items = new List<CartItemsDTO>();
        }
    }
}