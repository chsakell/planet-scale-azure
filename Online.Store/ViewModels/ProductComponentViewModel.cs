using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class ProductComponentViewModel
    {
        public string ProductComponentHeading { get; set; }
        public string ProductComponentDescription { get; set; }
        public List<ProductMediaViewModel> ProductMediaList { get; set; }
    }
}
