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
    }
}
