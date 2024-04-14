using MongoDB.Driver;
using System.Reflection;
using System.Xml.Linq;
using MongoDB.Bson;

namespace Project.Models
{
    public class DatabaseManipulator
    {
        private static IConfiguration? config;
        public static string? DATABASE_NAME;
        private static string? HOST;
        private static MongoServerAddress? address;
        private static MongoClientSettings? settings;
        private static MongoClient? client;
        public static IMongoDatabase? database;



        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
            var sections = config.GetSection("ConnectionStrings");
            DATABASE_NAME = sections.GetValue<string>("DatabaseName");
            HOST = sections.GetValue<string>("MongoConnection");
            address = new MongoServerAddress(HOST);
            settings = new MongoClientSettings() { Server = address };
            client = new MongoClient(settings);
            database = client.GetDatabase(DATABASE_NAME);

        }

        // Jos recordilla on _id päivittää
        // jos ei laittaa uuden tietokantaan
        // eli muista lähettää _id ettei tuu duplikaatteja
        // HUOM! EI KÄYTETÄ jos ei saa päällekirjoittaa esim. rekisteröinnissä
        // toiminta:
        //tarkistaa lähetetystä recordista onko _id tyhjä
        // on = insert uus, ei = update vanha
        //helppoa ja kivaa, dynaamista ja toimivaa
        public static T Save<T>(T record)
        {
            var mongotable = database.GetCollection<T>(typeof(T).Name);

            PropertyInfo idProp = typeof(T).GetProperty("_id");
            var idValue = (ObjectId)idProp.GetValue(record);

            if (IsObjectIdEmpty(idValue) == true)
            {
                try { mongotable.InsertOne(record); }
                catch { Console.WriteLine("Error while saving"); }
            } 
            
            else if (IsObjectIdEmpty(idValue) == false)
            {
                var filter = Builders<T>.Filter.Eq("_id", idValue);
                try { mongotable.ReplaceOne(filter, record, new ReplaceOptions { IsUpsert = true }); }
                catch { Console.WriteLine("Error while updating"); }
            }
             return record;
        }

        // Käytä, jos ei saa päällekirjoittaa, esim rekisteröinti
        public static T Register<T>(T record) 
        {
            var mongotable = database.GetCollection<T>(typeof(T).Name);

            PropertyInfo nameProp = typeof(T).GetProperty("Username");
            string nameValue = nameProp.GetValue(record)?.ToString();
            var filter = Builders<T>.Filter.Eq("Username", nameValue);

            var existingRecord = mongotable.Find(filter).FirstOrDefault();

            if (existingRecord == null)
            {
                try { mongotable.InsertOne(record); }
                catch { Console.WriteLine("Error while saving"); }
            } else if (existingRecord != null)
            {
                throw new Exception("Username already taken");
            }

            return record;
        }

        public static bool IsObjectIdEmpty(ObjectId objectid)
        {
            return objectid == ObjectId.Empty;
        }
    }
}
