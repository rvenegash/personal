using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");
            var database = dbClient.GetDatabase("dtv");


            otro(dbClient);

            var colRefClass = database.GetCollection<RefCustomerClass>("ref_customer_class");

            //database.ListCollections().ToList().ForEach(m => Console.WriteLine(m));
            var filter = Builders<RefCustomerClass>.Filter.Eq("_id", "1_1");

            //var refClass = colRefClass.Find(new BsonDocument()).ToList();
            var refClass = colRefClass.Find(filter).ToList();

        }

        static void otro(MongoClient dbClient)
        {
            var database = dbClient.GetDatabase("dtv");
            var colRefClass = database.GetCollection<BsonDocument>("ref_customer_class");

            //database.ListCollections().ToList().ForEach(m => Console.WriteLine(m));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "1_1");

            var refClass = colRefClass.Find(new BsonDocument()).ToList();
        }

        static void listDB(MongoClient dbClient)
        {
            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }
    }
}
