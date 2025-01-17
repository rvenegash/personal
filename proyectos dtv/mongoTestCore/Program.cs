
using MongoDB.Driver;

var dbClient = new MongoClient("mongodb://localhost:27017");
var database = dbClient.GetDatabase("dtv");


var colRefClass = database.GetCollection<RefCustomerStatus>("ref_customer_status");

//database.ListCollections().ToList().ForEach(m => Console.WriteLine(m));
var filter = Builders<RefCustomerStatus>.Filter.Eq("_id", "1_1");

//var refClass = colRefClass.Find(new BsonDocument()).ToList();
var refClass = colRefClass.Find(filter).ToList();

var x = refClass.Count;