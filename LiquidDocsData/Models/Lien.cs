using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiquidDocsData.Models;

[BsonIgnoreExtraElements]
[Table("Liens")]
public class Lien
{
    [Key]
    [BsonId]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public Guid PropertyId { get; set; }

    public string Description { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    [DataType(DataType.Text)]
    public LiquidDocsData.Enums.Lien.Positions LienPosition { get; set; }
}