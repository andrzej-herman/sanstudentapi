using StudentApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Entities.Authorization
{
    public class UserInfo
    {
        [Key]
        public string Id { get; set; }
        public string AlbumNumber { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateBlocked { get; set; }

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

        public string DisplayPassword
        {
            get
            {
                if (IsRegistered)
                    return "**********";
                else
                    return Cryptor.Decrypt(Password);
            }
        }

    }
}
