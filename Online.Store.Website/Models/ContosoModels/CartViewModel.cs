using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  Online.Store.Website.Models
{
    public class CartViewModel
    {
        public string UserId { get; set; }

        public List<CartItemViewModel> CartItems { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public CartViewModel()
        {
            this.CartItems = new List<CartItemViewModel>();
        }
    }
}