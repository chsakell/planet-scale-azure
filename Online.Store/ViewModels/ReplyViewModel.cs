using Microsoft.AspNetCore.Http;
using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class ReplyViewModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string MediaDescription { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }
    }
}
