package mongodb;

import org.bson.BsonReader;
import org.bson.BsonType;
import org.bson.BsonWriter;
import org.bson.Document;
import org.bson.codecs.Codec;
import org.bson.codecs.DecoderContext;
import org.bson.codecs.EncoderContext;

public class CustomerClassCodec implements Codec<CustomerClass> {
    private Codec<Document> documentCodec;

    @Override
    public CustomerClass decode(BsonReader bsonReader, DecoderContext decoderContext) {
        CustomerClass custClass = new CustomerClass();

        bsonReader.readStartDocument();
        while (bsonReader.readBsonType() != BsonType.END_OF_DOCUMENT) {
            String fieldName = bsonReader.readName();
            if (fieldName.equals("id")) {
                custClass.setid(bsonReader.readString());
            } else if (fieldName.equals("name")) {
                custClass.setNAME(bsonReader.readString());
            } else if (fieldName.equals("source_id")) {
                custClass.setSOURCE_ID(bsonReader.readString());
            } else if (fieldName.equals("_id")) {
                custClass.set_id(bsonReader.readString());
            }
        }
        bsonReader.readEndDocument();
        return custClass;
    }

    @Override
    public void encode(BsonWriter bsonWriter, CustomerClass value, EncoderContext encoderContext) {
        if (value != null) {
            Document document = new Document();

            String _id = value.get_id();
            String id = value.getid();
            String name = value.getNAME();
            String sourceId = value.getSOURCE_ID();

            if (null != _id) {
                document.put("_id", _id);
            }
            if (null != id) {
                document.put("id", id);
            }
            if (null != name) {
                document.put("name", name);
            }
            if (null != sourceId) {
                document.put("source_id", sourceId);
            }

            documentCodec.encode(bsonWriter, document, encoderContext);
        }
    }

    @Override
    public Class<CustomerClass> getEncoderClass() {
        return CustomerClass.class;
    }
}
