using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class UserCartViewModel
    {
        public CartDTO Cart { get; set; }
        public UserViewModel User { get; set; }
        public bool UseIdentity { get; set; }
    }
}
