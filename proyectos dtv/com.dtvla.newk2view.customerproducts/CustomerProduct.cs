using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class CustomerProduct
    {
        public string _id { get; set; }
        public string op_type { get; set; }
        public string op_ts { get; set; }
        public CustomerProductItem[] Product { get; set; }       
    }
}
