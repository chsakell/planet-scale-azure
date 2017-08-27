using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online.Store.Core.DTOs
{
    public class CartDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "cartitem")]
        public List<CartItemsDTO> CartItems { get; set; }

        [JsonProperty(PropertyName = "createddate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updateddate")]
        public DateTime? UpdateDate { get; set; }

        public CartDTO()
        {
            this.CartItems = new List<CartItemsDTO>();
        }

        public string UserName { get; set; }
    }
}