using MongoDB.Bson.Serialization.Attributes;

namespace com.dtvla.newk2view.customerproducts
{
    public class RefProductSpec
    {
        [BsonId]
        //[BsonElement("id")]
        public string _id { get; set; }

        // public string _id { get; set; }
        [BsonElement("id")]
        public string id { get; set; }

        [BsonElement("name")]
        public string NAME { get; set; }

        [BsonElement("description")]
        public string DESCRIPTION { get; set; }

        [BsonElement("category_id")]
        public string CATEGORY_ID { get; set; }

        [BsonElement("source_id")]
        public string SOURCE_ID { get; set; }
    }
}
