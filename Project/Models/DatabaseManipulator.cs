﻿using MongoDB.Driver;
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
        //helppoa ja kivaa, dynaamista ja toimivaa
        public static T Save<T>(T record)
        {
            var mongotable = database.GetCollection<T>(typeof(T).Name);

            PropertyInfo idProp = typeof(T).GetProperty("_id");
            var idValue = (ObjectId)idProp.GetValue(record);

            if (IsObjectIdEmpty(idValue))
            {
                try { mongotable.InsertOne(record); }
                catch { throw new Exception("Error while saving"); }
            }

            else if (!IsObjectIdEmpty(idValue))
            {
                var filter = Builders<T>.Filter.Eq("_id", idValue);
                try { mongotable.ReplaceOne(filter, record, new ReplaceOptions { IsUpsert = true }); }
                catch { throw new Exception("Error while updating"); }
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
                catch { throw new Exception("Error while saving"); }
            } else if (existingRecord != null)
            {   throw new Exception("Username already taken."); }

            return record;
        }
        public static T Delete<T>(T record)
        {
            var mongotable = database.GetCollection<T>(typeof(T).Name);
            PropertyInfo idProp = typeof(T).GetProperty("_id");
            var idValue = (ObjectId)idProp.GetValue(record);
            var filter = Builders<T>.Filter.Eq("_id", idValue);

            try
            {
                mongotable.DeleteOne(filter);
            }
            catch
            {
                throw new Exception("Error while deleting");
            }

            return record;

        }
        public static List<T> GetAll<T>(string table)
        {
            var mongotable = database.GetCollection<T>(table);
            return mongotable.Find(new BsonDocument()).ToList();
        }

        public static bool CheckIfUsernameExist(string username)
        {
            var mongotable = database.GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq("Username", username);   
            return mongotable.Find(filter).Any();
        }


        public static T GetById<T>(ObjectId ItemId, string table)
        {
            var mongotable = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", ItemId);
            var testi = mongotable.Find(filter).FirstOrDefault();
            return testi;
        }

        public static List<T> GetAllById<T>(ObjectId ItemId, string table)
        {
            var mongotable = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", ItemId);
            var testi = mongotable.Find(filter).ToList();
            return testi;
        }

        public static ObjectId GetUsersID(string username)
        {
            var mongotable = database.GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = mongotable.Find(filter).FirstOrDefault();
            return user._id;
        }

        public static bool CheckPassword(string username, string password)
        {
                var mongoCollection = database.GetCollection<BsonDocument>("User");
                var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
                var existingRecord = mongoCollection.Find(filter).FirstOrDefault();


            if (existingRecord != null)
            {
                byte[] salt = existingRecord["Salt"].AsBsonBinaryData.Bytes;
                string passwordHash = existingRecord["Password"].AsString;
                return PasswordHash.VerifyPassword(password, passwordHash, salt);
            }
            else {return false;}   
    }


         private static bool IsObjectIdEmpty(ObjectId objectid)
        {
            return objectid == ObjectId.Empty;
        }


    }
}
