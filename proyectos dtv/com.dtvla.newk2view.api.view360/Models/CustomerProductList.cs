using com.dtvla.newk2view.clases;
using MongoDB.Bson.Serialization.Attributes;

namespace com.dtvla.newk2view.api.view360.Models
{
    public class CustomerProductList
    {
        [BsonElement("Product")]
        public CustomerProductSingle[] Product { get; set; }
    }
}
