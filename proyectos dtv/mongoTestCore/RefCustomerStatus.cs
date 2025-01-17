using MongoDB.Bson.Serialization.Attributes;

public class RefCustomerStatus
{
    [BsonId]
    //[BsonElement("id")]
    public string _id { get; set; }

    // public string _id { get; set; }
    [BsonElement("id")]
    public string id { get; set; }

    [BsonElement("NAME")]
    public string NAME { get; set; }

    [BsonElement("SOURCE_ID")]
    public string SOURCE_ID { get; set; }
}