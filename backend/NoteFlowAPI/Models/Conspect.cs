using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;

namespace NoteFlowAPI.Models;

public class Conspect
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    [StringLength(300)]
    [BsonElement("title")]
    public required string Title { get; set; }

    [Required]
    [StringLength(300)]
    [BsonElement("type")]
    public required string Type { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("text")]
    public string? Text { get; set; }

    [BsonElement("dateTime")]
    public DateTime DateTime { get; set; }

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [Required]
    [BsonElement("isPrivate")]
    public bool IsPrivate { get; set; } = false;

    [BsonElement("allowedUsers")]
    public List<string> AllowedUserIds { get; set; } = new List<string>();
}
