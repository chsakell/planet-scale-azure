using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Store.Website.Models
{
    public class CommunityViewModel
    {
        public string id { get; set; }
        public DateTime PostDate { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public string UserName { get; set; }
        public string ImageName { get; set; }
        public int No_of_Posts { get; set; }
        public string ContentType { get; set; }
        public string ContentUrl { get; set; }
        public string UserImageName { get; set; }
        public List<CommunityViewModel> Response { get; set; }
        public string MediaDescription { get; set; }
        public string ParentId { get; set; }
        public int ResponseCount { get; set; }

        public CommunityViewModel()
        {
            this.Response = new List<CommunityViewModel>();
        }
    }
}