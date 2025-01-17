package KTableTest;

import Clases.RefCustomerClass;
import Clases.RefCustomerStatus;
import Clases.Stg_Customer;
import Clases.Stg_Customer_RF;
import SerializerUtil.JsonDeserializer;
import SerializerUtil.JsonSerializer;
import org.apache.kafka.common.serialization.Serde;
import org.apache.kafka.common.serialization.Serdes;

public final class CustomSerdes {
    private CustomSerdes() {}


    public static Serde<Stg_Customer> StgCustomer() {
        JsonSerializer<Stg_Customer> serializer = new JsonSerializer<>();
        JsonDeserializer<Stg_Customer> deserializer = new JsonDeserializer<>(Stg_Customer.class);
        return Serdes.serdeFrom(serializer, deserializer);
    }

    public static Serde<Stg_Customer_RF> StgCustomerRF() {
        JsonSerializer<Stg_Customer_RF> serializer = new JsonSerializer<>();
        JsonDeserializer<Stg_Customer_RF> deserializer = new JsonDeserializer<>(Stg_Customer_RF.class);
        return Serdes.serdeFrom(serializer, deserializer);
    }

    public static Serde<RefCustomerClass> RefCustomerClass() {
        JsonSerializer<RefCustomerClass> serializer = new JsonSerializer<>();
        JsonDeserializer<RefCustomerClass> deserializer = new JsonDeserializer<>(RefCustomerClass.class);
        return Serdes.serdeFrom(serializer, deserializer);
    }

    public static Serde<RefCustomerStatus> RefCustomerStatus() {
        JsonSerializer<RefCustomerStatus> serializer = new JsonSerializer<>();
        JsonDeserializer<RefCustomerStatus> deserializer = new JsonDeserializer<>(RefCustomerStatus.class);
        return Serdes.serdeFrom(serializer, deserializer);
    }
}
