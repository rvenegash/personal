using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class Stg_Agreement_Detail
    {
        public string table { get; set; }
        public string op_type { get; set; }
        public string op_ts { get; set; }
        public string current_ts { get; set; }
        public string pos { get; set; }
        public string[] primary_keys { get; set; }
        public Stg_Agreement_Detail_After after { get; set; }
    }
}
