using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafkaTest
{
    public class Stg_Customer_After
    {
        public string ID { get; set; }
        public string SOURCE_ID { get; set; }
        public string ENTITY_ID { get; set; }
        public string CHANGE_OPERATION_ID { get; set; }
        public string CHANGE_OPERATION_TIMESTAMP { get; set; }
        public string TYPE_ID { get; set; }
        public string STATUS_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string SEGMENTATION_ID { get; set; }
        public string CUSTOMER_SINCE { get; set; }
        public string EXEMPTION_CODE_ID { get; set; }
        public string EXEMPTION_SERIAL_NUMBER { get; set; }
        public string EXEMPTION_FROM { get; set; }
        public string REFERENCE_TYPE_ID { get; set; }
        public string IDENTIFICATION_ID { get; set; }
        public string INTERNET_PASSWORD_ENC { get; set; }
        public string INTERNET_PASSWORD_IV { get; set; }
        public string INTERNET_USER_NAME { get; set; }
        public string LANGUAGE_ID { get; set; }
        public string BIRTH_DATE { get; set; }
        public string BUSINESS_UNIT_ID { get; set; }
        public string MAGAZINES { get; set; }
        public string COUNTRY_ID { get; set; }
    }
}
