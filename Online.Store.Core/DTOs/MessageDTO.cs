using Newtonsoft.Json;

namespace Online.Store.Core.DTOs
{
    public class MessageDTO
    {
        [JsonProperty(PropertyName ="Status")]
        public bool Status { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }
    }
}