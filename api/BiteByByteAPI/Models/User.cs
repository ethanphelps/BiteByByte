using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BiteByByteAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("firstname")]
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        
        [BsonElement("lastname")]
        [JsonProperty("LastName")]
        public string LastName { get; set; }
        
        [BsonElement("email")]
        [JsonProperty("Email")]
        public string Email { get; set; }
    }
}