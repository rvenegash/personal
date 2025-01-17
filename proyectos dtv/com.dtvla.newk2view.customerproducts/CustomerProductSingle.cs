using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class CustomerProductSingle
    {
        public string _id { get; set; }
        public string op_type { get; set; }
        public string op_ts { get; set; }


        public string productKey { get; set; }
        public string ID { get; set; }
        public string CustomerKey { get; set; }
        public CategoryStatus productStatus { get; set; }
        public ProductSpecification productCategory { get; set; }
        public decimal productPrice { get; set; }
        public string contractStartDatetime { get; set; }
        public string contractEndDatetime { get; set; }
        public CategoryFinOp financeOption { get; set; }
        public string contractPeriod { get; set; }
        public string financialAccountID { get; set; }
        public string agreementID { get; set; }
        public CategoryMarkSeg marketSegment { get; set; }
        public ProductCharacteristicValue ProductCharacteristicValueCollection { get; set; }
        public string vendorId { get; set; }
        public CategoryComission commissionOption { get; set; }
    }
}
