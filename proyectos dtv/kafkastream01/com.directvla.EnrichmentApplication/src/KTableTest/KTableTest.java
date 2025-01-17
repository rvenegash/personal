package KTableTest;

import Clases.RefCustomerClass;
import Clases.RefCustomerStatus;
import Clases.Stg_Customer_RF;
import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.Topology;
import org.apache.kafka.streams.kstream.*;

import java.io.IOException;
import java.time.Duration;
import java.util.Properties;
import java.util.concurrent.CountDownLatch;

public class KTableTest {

    public static void main(String[] args) throws IOException {
        KTableTest ts = new KTableTest();
        Properties allProps = new Properties();

        allProps.put("bootstrap.servers", "localhost:9092");
        allProps.put("auto.offset.reset", "earliest");
        allProps.put("inputtopic", "STG.CUSTOMER_FLAT");
        allProps.put("refclasstopic", "REF_CUSTOMER_CLASS");
        allProps.put("refstatustopic", "REF_CUSTOMER_STATUS");
        allProps.put("outputtopic", "STG.CUSTOMER_RF");
        allProps.put("schema.registry.url", "localhost:9091");
        allProps.put("application.id", "stg-customer-enrichment");

        //allProps.put(StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG, Serdes.String().getClass());
        //allProps.put(StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, Serdes.String().getClass());
        //allProps.put(StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, Stg_Customer.class);
        //allProps.put(StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, CustomSerdes.StgCustomer().getClass());
        Topology topology = ts.buildTopology(allProps);

        final KafkaStreams streams = new KafkaStreams(topology, allProps);
        final CountDownLatch latch = new CountDownLatch(1);

        // Attach shutdown handler to catch Control-C.
        Runtime.getRuntime().addShutdownHook(new Thread("streams-shutdown-hook") {
            @Override
            public void run() {
                streams.close(Duration.ofSeconds(5));
                latch.countDown();
            }
        });

        try {
            streams.start();
            latch.await();
        } catch (Throwable e) {
            System.exit(1);
        }
        System.exit(0);
    }

    public Topology buildTopology(Properties allProps) {
        final StreamsBuilder builder = new StreamsBuilder();
        final String inputTopic = allProps.getProperty("inputtopic");
        final String refClassTopic = allProps.getProperty("refclasstopic");
        final String refStatusTopic = allProps.getProperty("refstatustopic");
        final String outputTopic = allProps.getProperty("outputtopic");
        //final CustomerJoiner joiner = new CustomerJoiner();


        //test para buscar en una ktable, no funcionó
/*
        KTable<String, RefCustomerClass> refCustomerClassKTable = builder.table(refTopic,
                Materialized.<String, RefCustomerClass, KeyValueStore<Bytes, byte[]>>as("ktable-store")
                .withKeySerde(Serdes.String())
                .withValueSerde(CustomSerdes.RefCustomerClass()));
        refCustomerClassKTable.filter((key, value) -> (value.equals("1_1")))
                .mapValues(value -> value)
                .toStream()
                .peek((key, value) -> System.out.println("Outgoing record - key " +key +" value " + value))
                .to("REF_CUSTOMER_CLASS_FILTER", Produced.with(Serdes.String(), CustomSerdes.RefCustomerClass()));
*/

        //copia de un topico a otro sin cambiar nada
        /*
        KStream<String, String> customers = builder.<String, String>stream(inputTopic);
        customers.to(outputTopic);
         */

        //cambia el formato del mensaje de stg_customer a stg_customer_rf
        //https://developer.confluent.io/tutorials/changing-serialization-format/kstreams.html
/*
        KStream<String, Stg_Customer> customers = builder.stream(inputTopic, Consumed.with(Serdes.String(), CustomSerdes.StgCustomer()));
        customers
                .map((key, customer) -> new KeyValue<>(key, joiner.apply(customer)))
                .to(outputTopic, Produced.with(Serdes.String(), CustomSerdes.StgCustomerRF()));
*/

        //usa foreign key para buscar en otro topico
        //https://www.confluent.io/blog/data-enrichment-with-kafka-streams-foreign-key-joins/
        KTable<String, RefCustomerClass> refCustomerClassKTable = builder.table(refClassTopic, Consumed.with(Serdes.String(), CustomSerdes.RefCustomerClass()));
        KTable<String, RefCustomerStatus> refCustomerStatusKTable = builder.table(refStatusTopic, Consumed.with(Serdes.String(), CustomSerdes.RefCustomerStatus()));
        KTable<String, Stg_Customer_RF> customers = builder.table(inputTopic, Consumed.with(Serdes.String(), CustomSerdes.StgCustomerRF()));
        KTable<String, Stg_Customer_RF> refCustomer = customers.leftJoin(
                refCustomerClassKTable,
                Stg_Customer_RF::getCLASS_KEY,
                (customer, refclass) -> new Stg_Customer_RF(customer, refclass)
        );
        refCustomer.toStream().to(outputTopic, Produced.with(Serdes.String(), CustomSerdes.StgCustomerRF()));

 /*
        KTable<String, RefCustomerClass> refCustomerClassKTable = builder.table(refTopic, Materialized.with(Serdes.String(), CustomSerdes.RefCustomerClass()));
        ValueJoiner<Stg_Customer_RF, RefCustomerClass, Stg_Customer_RF> enrichmentJoiner =
                ((stg_customer_rf, refCustomerClass) -> {
                    if (refCustomerClass != null) {
                        stg_customer_rf.setCLASS_NAME(refCustomerClass.getNAME());
                    }
                    else {
                        stg_customer_rf.setCLASS_NAME("*NOT_FOUND*");
                    }
                    return stg_customer_rf;
                }
                );

        KStream<String, Stg_Customer_RF> customersStr = builder.stream(inputTopic, Consumed.with(Serdes.String(), CustomSerdes.StgCustomerRF()));
       customersStr.leftJoin(
                        refCustomerClassKTable,
                        enrichmentJoiner,
                        Joined.with(Serdes.String(), CustomSerdes.StgCustomerRF(), CustomSerdes.RefCustomerClass()))
                .peek((key, value) -> System.out.println("Outgoing record - key " + key + " value " + value.getCLASS_ID()+ " name " + value.getCLASS_NAME()))
                .to(outputTopic, Produced.with(Serdes.String(), CustomSerdes.StgCustomerRF())
                );
        customersStr.join(
                        refCustomerClassKTable,
                        Stg_Customer_RF::getCLASS_ID,
                        (customer, refclass) -> new Stg_Customer_RF(customer, (RefCustomerClass)refclass))
                .peek((key, value) -> System.out.println("Outgoing record - key " + key + " value " + value.getCLASS_ID()+ " name " + value.getCLASS_NAME()))
                .to(outputTopic, Produced.with(Serdes.String(), CustomSerdes.StgCustomerRF())
                );
*/
        return builder.build();

    }

}
