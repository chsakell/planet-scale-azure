using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class ProductComponentViewModel
    {
        public ProductComponentViewModel()
        {
            this.Medias = new List<ProductMediaViewModel>();
        }
        public string ComponentType { get; set; }
        public string ComponentTitle { get; set; }
        public string ComponentDetail { get; set; }
        public List<ProductMediaViewModel> Medias { get; set; }
    }
}
