using MongoDB.Bson.Serialization.Attributes;

namespace com.dtvla.newk2view.clases
{
    public class Category
    {
        [BsonElement("id"), BsonIgnoreIfNull]
        public string id { get; set; }
        [BsonElement("name"), BsonIgnoreIfNull]
        public string name { get; set; }
        [BsonElement("code"), BsonIgnoreIfNull]
        public string code { get; set; }
    }

    public class CategoryFinOp : Category
    {

    }
    public class CategoryStatus : Category
    {

    }
    public class CategoryMarkSeg : Category
    {

    }
    public class CategoryComission : Category
    {

    }
    public class CategoryType : Category
    {

    }
}
