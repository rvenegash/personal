package EnrichmentApplication;

import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


public class LogWritingRecordsHandler implements ConsumerRecordsHandler<String, String> {
    Logger logger;

    public LogWritingRecordsHandler() {
        logger = LoggerFactory.getLogger(LogWritingRecordsHandler.class);
    }

    @Override
    public void process(final ConsumerRecords<String, String> consumerRecords) {
        /*consumerRecords.forEach(record->
                logger.info(String.format("Key: %s, Value: %s", record.key(), record.value()))
        );*/
        consumerRecords.forEach(record->
                System.out.print(String.format("Key: %s, Value: %s", record.key(), record.value()))
        );
    }
}