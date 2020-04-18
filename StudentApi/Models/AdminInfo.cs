using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class AdminInfo
    {
        public bool LoginResult { get; set; }
        public string ErrorUsername { get; set; }
        public string ErrorPassword { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsBlocked { get; set; }
        public string Initials { get; set; }

    }
}
