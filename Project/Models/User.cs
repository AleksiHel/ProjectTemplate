using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }


        [Required(ErrorMessage = "Username needs to between 3 and 10 characters.")]
        [MinLength(3)]
        [MaxLength(10)]
        public string ?Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ?Password { get; set; }
        public byte[] ?Salt { get; set; }
    }
}
