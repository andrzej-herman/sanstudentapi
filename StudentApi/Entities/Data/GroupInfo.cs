using StudentApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Entities.Data
{
    public class GroupInfo
    {
        [Key]
        public string Id { get; set; }

        public string Year { get; set; }

        public Semester Semester { get; set; }

        public string Name { get; set; }

        public string Subject { get; set; }

        public Day Day { get; set; }

        public string Time { get; set; }

        public Day LectureDay { get; set; }

        public string LectureTime { get; set; }

        public string LecturerName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }

    }
}
