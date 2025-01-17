using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJson
{
    public class responseObj {
        public data data { get; set; }
    }
    public class data
    {
        public GetCustomer360ViewResponse GetCustomer360ViewResponse { get; set; }
    }
    public class GetCustomer360ViewResponse
    {
        public responseMetadata responseMetadata { get; set; }
        public GetCustomer360ViewResult GetCustomer360ViewResult { get; set; }
    }
    public class responseMetadata
    { }
    public class GetCustomer360ViewResult
    {
        public Customer Customer { get; set; }
        public CustomerAgreement CustomerAgreement { get; set; }
        public CustomerInquiryList CustomerInquiryList { get; set; }
        public CustomerInvoiceList CustomerInvoiceList { get; set; }
        public CustomerWorkOrderList CustomerWorkOrderList { get; set; }
    }

    public class Customer
    {
        public string ID { get; set; }
        public validFor validFor { get; set; }
        public AddressList AddressList { get; set; }
        public CustomerAccountList CustomerAccountList { get; set; }
    }
    public class CustomerAgreement
    { }
    public class CustomerInquiryList
    {
        public CustomerInquiry[] CustomerInquiry { get; set; }
    }
    public class CustomerInvoiceList
    { }
    public class CustomerWorkOrderList
    {
        public WorkOrder[] WorkOrder { get; set; }
    }

    public class WorkOrder
    {
        public int ID { get; set; }
        public string CustomerKey { get; set; }
        public string interactionDate { get; set; }
        public DateTime interactionDateComplete { get; set; }
    }

    public class CustomerAccountList
    {
        public CustomerAccount[] CustomerAccount { get; set; }
    }
    public class CustomerAccount
    {
        public string ID { get; set; }
        public string CustomerKey { get; set; }
        public Category status { get; set; }
        public Category type { get; set; }
        public Category MethodOfPayment { get; set; }
        public Category DunningLevel { get; set; }
        public string name { get; set; }
        public DateTime DisconnectionDate { get; set; }
    }
    public class AddressList
    {
        public UrbanPropertyAddress[] UrbanPropertyAddress { get; set; }
    }

    public class CustomerInquiry
    {
        public string ID { get; set; }
        public string CustomerKey { get; set; }
        public string interactionDate { get; set; }
        public string description { get; set; }
        public DateTime interactionDateComplete { get; set; }
    }

    public class UrbanPropertyAddress
    {
        public string ID { get; set; }
        public string streetName { get; set; }
        public string city { get; set; }
        public string locality { get; set; }
        public string postCode { get; set; }

    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class validFor
    {
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }

    }
}
