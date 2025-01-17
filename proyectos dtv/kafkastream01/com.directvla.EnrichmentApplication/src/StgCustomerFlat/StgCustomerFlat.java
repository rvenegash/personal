package StgCustomerFlat;

import Clases.Stg_Customer;
import Clases.Stg_Customer_RF;
import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.KeyValue;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.Topology;
import org.apache.kafka.streams.kstream.Consumed;
import org.apache.kafka.streams.kstream.KStream;
import org.apache.kafka.streams.kstream.Produced;

import java.io.IOException;
import java.time.Duration;
import java.util.Properties;
import java.util.concurrent.CountDownLatch;

public class StgCustomerFlat {

    public static void main(String[] args) throws IOException {
        StgCustomerFlat ts = new StgCustomerFlat();
        Properties allProps = new Properties();

        allProps.put("bootstrap.servers", "localhost:9092");
        allProps.put("auto.offset.reset", "earliest");
        allProps.put("inputtopic", "STG.CUSTOMER");
        allProps.put("outputtopic", "STG.CUSTOMER_FLAT");
        allProps.put("schema.registry.url", "localhost:9091");
        allProps.put("application.id", "stg-customer-flatten");

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
        final String outputTopic = allProps.getProperty("outputtopic");

        //cambia el formato del mensaje de stg_customer a stg_customer_rf
        //https://developer.confluent.io/tutorials/changing-serialization-format/kstreams.html

        KStream<String, Stg_Customer> customers = builder.stream(inputTopic, Consumed.with(Serdes.String(), CustomFlatSerdes.StgCustomer()));
        customers
                .map((key, customer) -> new KeyValue<>(key, new Stg_Customer_RF(customer)))
                .to(outputTopic, Produced.with(Serdes.String(), CustomFlatSerdes.StgCustomerRF()));

        return builder.build();
    }
}
