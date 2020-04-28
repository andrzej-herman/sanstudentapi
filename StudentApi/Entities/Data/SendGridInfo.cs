using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Entities.Data
{
    public class SendGridInfo
    {
        [Key]
        public string Id { get; set; }

        public DateTime SendDate { get; set; }

        public int NumberOfSends { get; set; }

        public string DateComparer
        {
            get { return SendDate.ToString().Substring(0, 10); }
        }

    }
}
