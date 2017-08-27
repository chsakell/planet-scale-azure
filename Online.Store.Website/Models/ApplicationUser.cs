using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Online.Store.Website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string TwitterHandle { get; set; }
        public string ImageUrl { get; set; }
        public bool Notifications { get; set; }
        public bool ProductNotification { get; set; }
        public bool PostNotification { get; set; }
        public bool TweetNotification { get; set; }
    }
}
