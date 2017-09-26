using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Core.DTOs
{
    public class TopicDTO : PostDTO
    {
        [JsonProperty(PropertyName = "posts")]
        public List<PostDTO> Posts { get; set; }

        public TopicDTO()
        {
            Posts = new List<PostDTO>();
        }
    }
}
