using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Core.DTOs
{
    public class PagedTopics
    {
        public List<Topic> Topics { get; set; }
        public string ContinuationToken { get; set; }
    }
}
