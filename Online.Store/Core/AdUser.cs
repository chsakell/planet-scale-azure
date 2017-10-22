using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Core
{
    public class ActiveDirectoryUser
    {
        public string ObjectId { get; set; }
        public bool AccountEnabled { get; set; }
        public string CreationType { get; set; }
        public string UserPrincipalName { get; set; }
        public List<SignInName> SignInNames { get; set; }
    }

    public class SignInName
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
