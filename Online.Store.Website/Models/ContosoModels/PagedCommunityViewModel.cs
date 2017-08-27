using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class PagedCommunityViewModel
    {
        public int TotalPages { get; set; }

        public int SelectedPage { get; set; }

        public List<CommunityViewModel> Community { get; set; }

        public PagedCommunityViewModel()
        {
            this.Community = new List<CommunityViewModel>();
        }
    }
}