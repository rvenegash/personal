using com.dtvla.newk2view.clases;
using MongoDB.Bson.Serialization.Attributes;

namespace com.dtvla.newk2view.api.view360.Models
{
    public class GetCustomer360ViewResult
    {
        [BsonElement("Customer")]
        public Customer Customer { get; set; }
        [BsonElement("CustomerProductList")]
        public CustomerProductList CustomerProductList { get; set; }
    }
}
