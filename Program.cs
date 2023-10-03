using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongodbTest
{
    class Program
    {
        public class Sales
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? Id { get; set; }
            public string item { get; set; } = null!;
            public double price { get; set; } = 0.0!;
            public int quantity { get; set; } = 0!;
            public DateTime date { get; set; }
        }
        static void Main(string[] args)
        {
            try
            {
                //連線資訊 
                var connString = "mongodb://admin:admin@10.96.153.4:27017/?authSource=mvix&readPreference=primary&ssl=false&directConnection=true";
                MongoClient client = new MongoClient(connString);

                //顯示所有的DB
                var allDatabases = client.ListDatabases().ToList();
                Console.WriteLine(string.Join(", ", allDatabases));

                var collection = client.GetDatabase("mvix").GetCollection<Sales>("sales");

                //select * from mvix.sales
                var result = collection.Find(_ => true).ToList();
                ConsoleLog("select", result);

                //select * from mvix.sales where price >= 10
                var filter1 = collection.Find(Builders<Sales>.Filter.Gte(s => s.price, 10.0)).ToList();
                ConsoleLog("select/where", filter1);

                //insert
                var addItem = new Sales() { item = "sss", price = 100, quantity = 5, date = DateTime.Now };
                collection.InsertOne(addItem);
                var filter2 = collection.Find(Builders<Sales>.Filter.Eq(s => s.item, "sss")).ToList();
                ConsoleLog("insert", filter2);

                //update
                var updatefilter = Builders<Sales>.Filter.Eq("item", "sss");
                var updateItem = Builders<Sales>.Update.Set("price", 50);
                collection.UpdateOne(updatefilter, updateItem);
                var filter3 = collection.Find(Builders<Sales>.Filter.Eq(s => s.item, "sss")).ToList();
                ConsoleLog("update", filter3);

                //delete
                var delfilter = Builders<Sales>.Filter.Eq("item", "sss");
                collection.DeleteMany(delfilter);
                var filter4 = collection.Find(_ => true).ToList();
                ConsoleLog("delete", filter4);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
        public static void ConsoleLog(string method, List<Sales> data)
        {
            Console.WriteLine($"{method}");
            foreach (var item in data)
            {
                Console.WriteLine($"Id: {item.Id}, Item: {item.item}, Price: {item.price}, Quantity: {item.quantity}, Date: {item.date}");
            }
            Console.WriteLine("\n");

        }

    }



}