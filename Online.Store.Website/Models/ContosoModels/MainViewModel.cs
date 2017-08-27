using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class MainViewModel
    {
        public List<NewsViewModel> News { get; set; }
        public List<CommunityViewModel> Community { get; set; }
        public List<Online.Store.Website.Models.CarouselItemViewModel> CarousalItems { get; set; }
    }
}