using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class ProductComponentViewModel
    {
        public string ProductComponentHeading { get; set; }
        public string ProductComponentDescription { get; set; }
        public List<ProductMediaViewModel> ProductMediaList { get; set; }
    }
}