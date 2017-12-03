using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Core.DTOs
{
    public class SearchResultDTO
    {
        public List<CommunityDTO> Posts { get; set; }

        public List<Product> Product { get; set; }

        public SearchResultDTO()
        {
            this.Posts = new List<CommunityDTO>();
            this.Product = new List<Product>();
        }
    }
}
