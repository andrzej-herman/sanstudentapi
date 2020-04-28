using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class AdminModel
    {
        public string DisplayName { get; set; }
        public string Initials { get; set; }
        public string Password { get; set; }
        public int SendGrid { get; set; }
    }
}
