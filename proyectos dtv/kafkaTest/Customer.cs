using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafkaTest
{
    public class Customer
    {
        public string _id { get; set; }
        public string op_type { get; set; }
        public string op_ts { get; set; }

        public CategoryStatus status { get; set; }
        public string name { get; set; }
        public TimePeriodSince validFor { get; set; }

        public string AddressList { get; set; }
        public string ContactableVia { get; set; }

        public CategoryType CategorizedBy { get; set; }
        public string ID { get; set; }
        public string customerRank { get; set; }
        public string customerRankName { get; set; }

        public string CustomerAccountList { get; set; }

        public Individual IndividualRole { get; set; }
        public string BusinessUnitId { get; set; }
        public string magazines { get; set; }
        public string agreementId { get; set; }
        public string SegmentationKey { get; set; }
        public string segmentationKeyName { get; set; }
    }
}
