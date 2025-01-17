using AutoMapper;
using Confluent.Kafka;
using kafkaTest;
using MongoDB.Driver;
using System.Text.Json;

var mapper = new Mapper(createMap());

var dbClient = new MongoClient("mongodb://localhost:27017");
var database = dbClient.GetDatabase("dtv");

var colCustomer = database.GetCollection<Customer>("customer");

var colRefCustStatus = database.GetCollection<RefCustomerStatus>("ref_customer_status");
var colRefCustType = database.GetCollection<RefCustomerStatus>("ref_customer_type");
var colRefCustClass = database.GetCollection<RefCustomerStatus>("ref_customer_class");


var conf = new ConsumerConfig
{
    GroupId = "test-consumer-group",
    BootstrapServers = "localhost:9092",
    // Note: The AutoOffsetReset property determines the start offset in the event
    // there are not yet any committed offsets for the consumer group for the
    // topic/partitions of interest. By default, offsets are committed
    // automatically, so in this example, consumption will only start from the
    // earliest message in the topic 'my-topic' the first time you run the program.
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
{
    c.Subscribe("STG.CUSTOMER");

    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true; // prevent the process from terminating.
        cts.Cancel();
    };

    try
    {
        while (true)
        {
            try
            {
                var cr = c.Consume(cts.Token);
                Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");

                var cust = JsonSerializer.Deserialize<Stg_Customer>(cr.Value);

                if (!string.IsNullOrEmpty(cust.after.ENTITY_ID))
                {
                   /* var customer = mapper.Map<Customer>(cust);
                    customer.IndividualRole.IdentifiedBy = new IndividualIdentificationCollection();
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications = new IndividualIdentification[2];
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications[0] = new IndividualIdentification();
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications[1] = new IndividualIdentification();
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications[0].scan = cust.after.REFERENCE_TYPE_ID;
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications[0].cardNr = cust.after.IDENTIFICATION_ID;
                    customer.IndividualRole.IdentifiedBy.IndividualIdentifications[1].scan = cust.after.INTERNET_USER_NAME;

                    customer.status.name = getCustStatusName(cust.after.SOURCE_ID, customer.status.id);
                    customer.CategorizedBy.name = getCustTypeName(cust.after.SOURCE_ID, customer.CategorizedBy.id);
                    customer.customerRankName = getCustClassName(cust.after.SOURCE_ID, customer.customerRank);

                    try
                    {
                        colCustomer.InsertOne(customer);
                    }
                    catch (Exception ex)
                    {
                        var x = ex.Message;
                    }
                   */

                    UpdateOptions options = new UpdateOptions();
                    options.IsUpsert = true;

                    //FilterDefinition<Customer> filter = "{ _id: \"\" }";
                    //FilterDefinition<Customer> filter = (FilterDefinition<Customer>)(new Customer() { _id = customer._id });

                    var builder = Builders<Customer>.Filter;
                    var filter = builder.Eq("_id", cust.after.ENTITY_ID);

                    var update = Builders<Customer>.Update
                        .Set(m => m.op_ts, cust.op_ts)
                        .Set(m => m.op_type, cust.op_type)
                        .Set(m => m.customerRank, cust.after.CLASS_ID)
                        .Set(m => m.customerRankName, getCustClassName(cust.after.SOURCE_ID, cust.after.CLASS_ID))
                        .Set(m => m.CategorizedBy.id, cust.after.TYPE_ID)
                        .Set(m => m.CategorizedBy.name, getCustClassName(cust.after.SOURCE_ID, cust.after.TYPE_ID))
                        .Set(m => m.status.id, cust.after.STATUS_ID)
                        .Set(m => m.status.name, getCustStatusName(cust.after.SOURCE_ID, cust.after.STATUS_ID))
                        .Set(m => m.SegmentationKey, cust.after.SEGMENTATION_ID)
                        .Set(m => m.BusinessUnitId, cust.after.BUSINESS_UNIT_ID)
                        .Set(m => m.magazines, cust.after.MAGAZINES)
                        .Set(m => m.IndividualRole.aliveDuring.startDateTime, cust.after.BIRTH_DATE)
                        .Set(m => m.validFor.startDateTime, cust.after.CUSTOMER_SINCE)
                        .Set(m => m.IndividualRole.IdentifiedBy.IndividualIdentifications[0].scan, cust.after.REFERENCE_TYPE_ID)
                        .Set(m => m.IndividualRole.IdentifiedBy.IndividualIdentifications[0].cardNr, cust.after.IDENTIFICATION_ID)
                        .Set(m => m.IndividualRole.IdentifiedBy.IndividualIdentifications[1].scan, cust.after.INTERNET_USER_NAME);

                    colCustomer.UpdateOne(filter, update, options);

                }

            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }
        }
    }
    catch (OperationCanceledException)
    {
        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        c.Close();
    }
}

MapperConfiguration createMap()
{
    var config = new MapperConfiguration(
        cfg =>
           {
               cfg.AddProfile(new DtvProfile());
           }
        );

    return config;
}

string getCustStatusName(string country_id, string statusId)
{
    var filter = Builders<RefCustomerStatus>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefCustStatus.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}


string getCustTypeName(string country_id, string statusId)
{
    var filter = Builders<RefCustomerStatus>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefCustType.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}


string getCustClassName(string country_id, string statusId)
{
    var filter = Builders<RefCustomerStatus>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefCustClass.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}