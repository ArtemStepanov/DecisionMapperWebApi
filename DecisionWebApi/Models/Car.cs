using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DecisionWebApi.Models
{
    public class Car : IEntity
    {
        [Column("id")] public int Id { get; set; }

        [Required]
        [Column("name")]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [Column("description")]
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; }
    }
}