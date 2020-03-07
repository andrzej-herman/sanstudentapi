using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class StudentGroup
    {
        public string Id { get; set; }
        public string Season { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string Lecture { get; set; }
        public List<Student> Students { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public int NumberOfStudents
        {
            get
            {
                if (Students == null)
                    return 0;
                else
                    return Students.Count;
            }
        } 
    }
}
