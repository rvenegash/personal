package mongodb;

import com.mongodb.BasicDBObject;
import com.mongodb.MongoClientSettings;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import org.bson.codecs.configuration.CodecRegistries;
import org.bson.codecs.configuration.CodecRegistry;

public class mongodbtest {

    public static void main(String[] args) throws Exception {
        CodecRegistry codecRegistry = CodecRegistries.fromRegistries(
                CodecRegistries.fromCodecs(new CustomerClassCodec()),
                MongoClientSettings.getDefaultCodecRegistry());

        MongoClient mongo = (MongoClient) MongoClients.create("mongodb://localhost:27017/?directConnection=true");

        MongoDatabase db = mongo.getDatabase("dtv");
        MongoCollection<CustomerClass> colClass = db.getCollection("ref_customer_class", CustomerClass.class).withCodecRegistry(codecRegistry);
        BasicDBObject query = new BasicDBObject();
        query.put("_id", new BasicDBObject("$eq", "1_1"));
        for (CustomerClass doc : colClass.find(query)) {
            System.out.println(doc.get_id() + " " + doc.getid() + " " +doc.getNAME() + " " +doc.getSOURCE_ID());
        }


       /* MongoCollection<Document> colClass = db.getCollection("ref_customer_class");
        BasicDBObject query = new BasicDBObject();
        query.put("_id", new BasicDBObject("$eq", "1_1"));

        for (Document doc : colClass.find(query)) {
            System.out.println(doc.toJson());
        }
        */
    }
}
