using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            this.Components = new List<ProductComponentViewModel>();
        }
        public string Id { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Model { get; set; }
        public string SKU { get; set; }
        public List<ProductComponentViewModel> Components { get; set; }
    }
}
