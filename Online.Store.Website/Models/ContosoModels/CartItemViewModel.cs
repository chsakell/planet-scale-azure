using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  Online.Store.Website.Models
{
    public class CartItemViewModel
    {
        public string Id { get; set; }

        public int Qty { get; set; }

        public ProductsViewModel Products { get; set; }

        public CartItemViewModel()
        {
            this.Qty = 1;
            this.Products = new ProductsViewModel();
        }
    }
}