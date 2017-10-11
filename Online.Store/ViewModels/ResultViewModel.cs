using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.ViewModels
{
    public class ResultViewModel
    {
        public Result Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public enum Result
    {
        SUCCESS = 1,
        ERROR = 2
    }
}
