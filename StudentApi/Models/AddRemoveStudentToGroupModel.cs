using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Models
{
    public class AddRemoveStudentToGroupModel
    {
        public string Student { get; set; }
        public string Group { get; set; }

        public override string ToString()
        {
            int fp = Student.IndexOf('(');
            int sp = Student.IndexOf(')');
            int count = sp - fp;
            return Student.Substring(fp + 1, count - 1);
        }
    }
}
