using System;

namespace Online.Store.Core.DTOs
{
    public class PostDTO
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string MediaDescription { get; set; }
        public string ContentUrl { get; set; }
        public string ContentType { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string UserName { get; set; }
        //public string UserImage { get; set; }
        public string UserId { get; set; }
    }
}
