using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Core.DTOs
{
    public class PagedCommunityDto
    {
        public int TotalPages { get; set; }

        public int SelectedPage { get; set; }

        public List<CommunityDTO> Community { get; set; }
    }
}
