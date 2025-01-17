using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class ProductCharacteristicValue
    {

        public  ProductCharacteristic[] ProductCharValue { get; set; }
    }

    public class ProductCharacteristic
    {
        public string value { get; set; }
        public Category ProductSpecCharacteristic { get; set; }
    }
}
