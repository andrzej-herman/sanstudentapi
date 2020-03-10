using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string AlbumNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsBlocked { get; set; }
        public string Initials { get; set; }
        public string Login { get; set; }
        public string GitHubLogin { get; set; }

        public string GitHub
        {
            get
            {
                if (GitHubLogin == null)
                    return null;
                else
                    return @"https://github.com/" + GitHubLogin;
            }
        }
    }
}
