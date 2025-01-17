using AutoMapper;
using com.dtvla.newk2view.customerproducts;
using Confluent.Kafka;
using MongoDB.Driver;
using System.Text.Json;

var mapper = new Mapper(createMap());

var dbClient = new MongoClient("mongodb://localhost:27017");
var database = dbClient.GetDatabase("dtv_AR");

//var colProducts = database.GetCollection<CustomerProductSingle>("CustomerProductList");
var colProducts = database.GetCollection<CustomerProductSingle>("CustomerProductListSingle");

var colRefProdStatus = database.GetCollection<RefCategory>("ref_agreement_detail_status");
var colRefFinOption = database.GetCollection<RefCategory>("ref_finance_option");
var colRefProdCategory = database.GetCollection<RefCategory>("ref_product_category");
var colRefProdSpecification = database.GetCollection<RefProductSpec>("ref_product_specification");

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
    c.Subscribe("STG.AGREEMENT_DETAIL");

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

                var prod = JsonSerializer.Deserialize<Stg_Agreement_Detail>(cr.Value);

                if (!string.IsNullOrEmpty(prod.after.ENTITY_ID))
                {
                    var modo = "A";

                    if (modo == "A")
                    {
                        var builder = Builders<CustomerProductSingle>.Filter;
                        var filter = builder.And(
                            builder.Eq("_id", prod.after.AGREEMENT_ITEM_ID)
                            );
                        var docProduct = colProducts.Find(filter).FirstOrDefault();
                        if (docProduct is null)
                        {
                            //crear
                            var product = mapper.Map<CustomerProductSingle>(prod);

                            product.productStatus.name = getProductStatusName(prod.after.SOURCE_ID, prod.after.PRODUCT_STATUS_ID);
                            product.productCategory = getProduct(prod.after.SOURCE_ID, prod.after.PRODUCT_SPECIFICATION_ID);
                            product.financeOption.name = getFinOptionName(prod.after.SOURCE_ID, prod.after.FINANCE_OPTION_ID);

                            product.ProductCharacteristicValueCollection = new ProductCharacteristicValue();
                            product.ProductCharacteristicValueCollection.ProductCharValue = new ProductCharacteristic[3];
                            product.ProductCharacteristicValueCollection.ProductCharValue[0] = new ProductCharacteristic()
                            {
                                value = prod.after.CHARGE_PERIOD,
                                ProductSpecCharacteristic = new Category() { name = "ChargePeriod" }
                            };
                            product.ProductCharacteristicValueCollection.ProductCharValue[1] = new ProductCharacteristic()
                            {
                                value = prod.after.CHARGE_UNTIL_DATE,
                                ProductSpecCharacteristic = new Category() { name = "ChargeUntilDate" }
                            };
                            product.ProductCharacteristicValueCollection.ProductCharValue[2] = new ProductCharacteristic()
                            {
                                value = "1",
                                ProductSpecCharacteristic = new Category() { name = "Quantity" }
                            };

                            try
                            {
                                colProducts.InsertOne(product);
                            }
                            catch (Exception ex)
                            {
                                var x = ex.Message;
                            }
                        }
                        else
                        {
                            //validar fecha TS
                            DateTime docTS = DateTime.MinValue;
                            DateTime prodTS = DateTime.MinValue;

                            if (DateTime.TryParse(docProduct.op_ts, out docTS) && DateTime.TryParse(prod.op_ts, out prodTS))
                            {
                                if (docTS < prodTS)
                                {
                                    //actualizar

                                    UpdateOptions options = new UpdateOptions();

                                    var update = Builders<CustomerProductSingle>.Update
                                        .Set(m => m.op_ts, prod.op_ts)
                                        .Set(m => m.op_type, prod.op_type)
                                        .Set(m => m.productKey, prod.after.AGREEMENT_ITEM_ID)
                                        .Set(m => m.ID, prod.after.AGREEMENT_ITEM_ID)
                                        .Set(m => m.CustomerKey, prod.after.CUSTOMER_ID)
                                        .Set(m => m.productStatus.id, prod.after.PRODUCT_STATUS_ID)
                                        .Set(m => m.productStatus.name, getProductStatusName(prod.after.SOURCE_ID, prod.after.PRODUCT_STATUS_ID))
                                        .Set(m => m.productCategory, getProduct(prod.after.SOURCE_ID, prod.after.PRODUCT_SPECIFICATION_ID))
                                        .Set(m => m.productPrice, 0)
                                        .Set(m => m.contractStartDatetime, prod.after.FROM_DATE)
                                        .Set(m => m.contractEndDatetime, prod.after.TO_DATE)
                                        .Set(m => m.financeOption.id, prod.after.FINANCE_OPTION_ID)
                                        .Set(m => m.financeOption.name, getFinOptionName(prod.after.SOURCE_ID, prod.after.FINANCE_OPTION_ID))
                                        .Set(m => m.contractPeriod, prod.after.CONTRACT_PERIOD)
                                        .Set(m => m.financialAccountID, prod.after.ACCOUNT_ID)
                                        .Set(m => m.agreementID, prod.after.AGREEMENT_ID)
                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[0].value, prod.after.CHARGE_PERIOD)
                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[0].ProductSpecCharacteristic.name, "ChargePeriod")

                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[1].value, prod.after.CHARGE_UNTIL_DATE)
                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[1].ProductSpecCharacteristic.name, "ChargeUntilDate")

                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[2].value, "1")
                                        .Set(m => m.ProductCharacteristicValueCollection.ProductCharValue[2].ProductSpecCharacteristic.name, "Quantity")
                                        ;


                                    //.Set(m => m.marketSegment.id, prod.after.ma)
                                    //.Set(m => m.vendorId
                                    //.Set(m => m.commissionOption, prod.after.co)

                                    colProducts.UpdateOne(filter, update, options);
                                }
                            }
                        }


                    }
                    else
                    {




                        UpdateOptions options = new UpdateOptions();
                        options.IsUpsert = true;

                        var builder = Builders<CustomerProduct>.Filter;
                        var filter = builder.And(
                            builder.Eq("_id", prod.after.ENTITY_ID),
                            builder.ElemMatch(m => m.Product, f => f.productKey == prod.after.AGREEMENT_ITEM_ID)
                            );

                        var productItem = mapper.Map<CustomerProductItem>(prod);




                        var update = Builders<CustomerProduct>.Update
                            .Set(m => m.op_ts, prod.op_ts)
                            .Set(m => m.op_type, prod.op_type)
                            .AddToSet("Product", productItem)
                            ;



                        //.Set(m => m.Product[-1].productKey, prod.after.AGREEMENT_ITEM_ID)
                        //.Set(m => m.Product[-1].ID, prod.after.AGREEMENT_ITEM_ID)
                        //.Set(m => m.Product[-1].CustomerKey, prod.after.CUSTOMER_ID)
                        //.Set(m => m.Product[-1].productStatus.id, prod.after.PRODUCT_STATUS_ID)
                        //.Set(m => m.Product[-1].productStatus.name, getProductStatusName(prod.after.SOURCE_ID, prod.after.PRODUCT_STATUS_ID))
                        //.Set(m => m.Product[-1].productCategory, getProduct(prod.after.SOURCE_ID, prod.after.PRODUCT_SPECIFICATION_ID))
                        //.Set(m => m.Product[-1].productPrice, 0)
                        //.Set(m => m.Product[-1].contractStartDatetime, prod.after.FROM_DATE)
                        //.Set(m => m.Product[-1].contractEndDatetime, prod.after.TO_DATE)
                        //.Set(m => m.Product[-1].financeOption.id, prod.after.FINANCE_OPTION_ID)
                        //.Set(m => m.Product[-1].financeOption.name, getFinOptionName(prod.after.SOURCE_ID, prod.after.FINANCE_OPTION_ID))
                        //.Set(m => m.Product[-1].contractPeriod, prod.after.CONTRACT_PERIOD)
                        //.Set(m => m.Product[-1].financialAccountID, prod.after.ACCOUNT_ID)
                        //.Set(m => m.Product[-1].agreementID, prod.after.AGREEMENT_ID)
                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[0].value, prod.after.CHARGE_PERIOD)
                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[0].ProductSpecCharacteristic.name, "ChargePeriod")

                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[1].value, prod.after.CHARGE_UNTIL_DATE)
                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[1].ProductSpecCharacteristic.name, "ChargeUntilDate")

                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[2].value, "1")
                        //.Set(m => m.Product[-1].ProductCharacteristicValueCollection.ProductCharValue[2].ProductSpecCharacteristic.name, "Quantity")

                        //.Set(m => m.Product[-1].marketSegment.id, prod.after.ma)
                        //.Set(m => m.Product[-1].vendorId
                        //.Set(m => m.Product[-1].commissionOption, prod.after.co)

                        //colProducts.UpdateOne(filter, update, options);
                    }
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

ProductSpecification getProduct(string country_id, string product_id)
{
    var res = new ProductSpecification()
    {
        id = product_id,
        description = "",
        type = new CategoryType()
        {
            id = "",
            name = ""
        }
    };
    var spec = getProductSpecification(country_id, product_id);
    res.description = spec.NAME;
    res.type.id = spec.CATEGORY_ID;
    res.type.name = getProductCategory(country_id, spec.CATEGORY_ID);

    return res;
}


string getProductStatusName(string country_id, string statusId)
{
    var filter = Builders<RefCategory>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefProdStatus.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}

string getFinOptionName(string country_id, string statusId)
{
    var filter = Builders<RefCategory>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefFinOption.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}


string getProductCategory(string country_id, string statusId)
{
    var filter = Builders<RefCategory>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefProdCategory.Find(filter).FirstOrDefault();

    return refClass is null ? "" : refClass.NAME;
}

RefProductSpec getProductSpecification(string country_id, string statusId)
{
    var filter = Builders<RefProductSpec>.Filter.Eq("_id", string.Format("{0}_{1}", statusId, country_id));
    var refClass = colRefProdSpecification.Find(filter).FirstOrDefault();

    return refClass;
}
