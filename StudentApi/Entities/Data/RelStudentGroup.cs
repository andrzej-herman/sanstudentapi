using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Entities.Data
{
    public class RelStudentGroup
    {
        [Key]
        public string GroupId { get; set; }

        [Key]
        public string StudentId { get; set; }
    }
}
