using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Core.DTOs
{
    public class CartItemsDTO
    {
        [JsonProperty(PropertyName = "product")]
        public string Product { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "productTitle")]
        public string ProductTitle { get; set; }

        [JsonProperty(PropertyName = "productImage")]
        public string ProductImage { get; set; }
    }
}
