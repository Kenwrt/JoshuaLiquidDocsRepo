using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LiquidDocsData.Models;

[BsonIgnoreExtraElements]
public class DocumentLibrary
{
    [Key]
    [BsonIgnoreIfDefault]
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public String Description { get; set; }

    public bool IsUsingDefaultTemplate { get; set; } = true;

    public string MasterTemplate { get; set; } = "Master Default Template";

    public byte[] MasterTemplateBytes { get; set; }

    public List<LiquidDocsData.Models.Document> Documents { get; set; } = new();

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}