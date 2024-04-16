using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }


        [Required(ErrorMessage = "Enter valid username")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        [MaxLength(10, ErrorMessage = "Username must be under 10 characters long")]
        public string? Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(20, ErrorMessage = "Password must be less than 20 characters long.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public byte[] ?Salt { get; set; }

        public bool? IsAdmin { get; set; }
    }
}
