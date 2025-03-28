namespace NoteFlowAPI.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a user in the system, stored in MongoDB.
/// </summary>

public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address, used for authentication.
    /// </summary>

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password hash, stored securely.
    /// </summary>

    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's role, defaults to "User".
    /// </summary>

    public string Role { get; set; } = "User";
}

