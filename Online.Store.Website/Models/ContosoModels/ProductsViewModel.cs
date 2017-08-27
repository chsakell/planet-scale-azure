using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class ProductsViewModel
    {
        public string ProductId { get; set; }
        public double ProductPrice { get; set; }
        public string ProductHeading { get; set; }
        public string Description { get; set; }
        public string ProductURL { get; set; }
        public List<ProductComponentViewModel> ProductComponentsList { get; set; }
    }
}