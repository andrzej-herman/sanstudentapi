using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class Student
    {
        public string Id { get; set; }
        public string AlbumNumber { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateBlocked { get; set; }
        public string Login { get; set; }
        public string GitHubLogin { get; set; }
        public List<ShortGroupModel> Groups { get; set; }
        public string AvatarPath { get; set; }

        public string Initials
        {
            get
            {
                if (FirstName != null && LastName != null)
                    return FirstName.Substring(0, 1).ToUpper() + LastName.Substring(0, 1).ToUpper();
                else
                    return "";
            }
        }

        public string GitHubUrl
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
