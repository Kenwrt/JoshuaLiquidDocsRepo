using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LiquidDocsData.Models;

[BsonIgnoreExtraElements]
public class MergedDocument
{
    [Key]
    [BsonIgnoreIfDefault]
    [BsonId]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string MergeValues { get; set; }

    public Guid DocumentTemplateId { get; set; }

    public byte[] DocumentBytes { get; set; }

    public byte[] MergedDocumentBytes { get; set; }

    public Guid LoanAgreementId { get; set; }

    public string? MergedDocumentPath { get; set; }

    public string HiddenTagName { get; set; } = "LiquidDocsTag";

    public string HiddenTagValue { get; set; }

    public bool IsActive { get; set; } = true;
}