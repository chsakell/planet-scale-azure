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
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "qty")]
        public int Qty { get; set; }

        [JsonProperty(PropertyName = "products")]
        public ProductDTO Products { get; set; }

        public CartItemsDTO()
        {
            this.Qty = 1;
            this.Products = new ProductDTO();
        }
    }
}
