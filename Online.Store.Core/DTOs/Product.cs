using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Online.Store.Core.DTOs
{
    public class Product : ProductInfo
    {
        public Product()
        {
            this.Components = new List<ProductComponent>();
        }

        [JsonProperty(PropertyName="components")]
        public List<ProductComponent> Components { get; set; }
    }
}
