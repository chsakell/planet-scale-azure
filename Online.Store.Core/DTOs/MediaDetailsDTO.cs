using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Core.DTOs
{
    public class MediaDetailsDTO : MessageDTO
    {
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
    }
}
