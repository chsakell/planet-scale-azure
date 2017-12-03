using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Core.DTOs
{
    public class Topic : Post
    {
        [JsonProperty(PropertyName = "posts")]
        public List<Post> Posts { get; set; }

        public Topic()
        {
            Posts = new List<Post>();
        }
    }
}
