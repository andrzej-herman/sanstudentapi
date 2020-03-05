using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Helpers
{
    public class OperationResult
    {
        public bool Result { get; set; }
        public string Content { get; set; }
        public string Error { get; set; }
    }
}
