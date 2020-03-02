using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models.Authorization
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
        public bool IsRegistered { get; set; }
        public bool IsBlocked { get; set; }

    }
}
