using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BiteByByteAPI.Entities
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
        
        [BsonElement("username")]
        [JsonProperty("Username")]
        public string Username { get; set; }
        
        [BsonElement("passwordhash")]
        [JsonProperty("PasswordHash")]
        public byte[] PasswordHash { get; set; }
        
        [BsonElement("passwordsalt")]
        [JsonProperty("PasswordSalt")]
        public byte[] PasswordSalt { get; set; }
    }
}