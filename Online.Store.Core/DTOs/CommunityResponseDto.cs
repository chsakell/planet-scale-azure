using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Core.DTOs
{
    public class CommunityResponseDto
    {
        public int TotalPages { get; set; }

        public int SelectedPage { get; set; }

        public CommunityDTO Community { get; set; }

        public CommunityResponseDto()
        {
            this.Community = new CommunityDTO();
        }
    }
}
