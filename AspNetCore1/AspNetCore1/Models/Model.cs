using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore1.Models
{
    public class Model
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
        public string names { get; set; }
        public string photo { get; set; }
        public int year { get; set; }
        public string type { get; set; }
        public string makerId { get; set; }
        public string urls { get; set; }
    }
}
