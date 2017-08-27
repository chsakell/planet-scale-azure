using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class CommunityResponseViewModel
    {
        public int TotalPages { get; set; }

        public int SelectedPage { get; set; }

        public CommunityViewModel Community { get; set; }

        public CommunityResponseViewModel()
        {
            this.Community = new CommunityViewModel();
        }
    }
}