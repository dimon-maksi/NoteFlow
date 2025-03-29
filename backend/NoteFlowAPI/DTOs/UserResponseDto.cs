namespace NoteFlowAPI.DTOs;

/// <summary>
/// Data Transfer Object for user information responses.
/// </summary>

public class UserResponseDto
{
    /// <summary>
    /// Gets or sets the user's unique identifier.
    /// </summary>

    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>

    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's role, defaults to "User".
    /// </summary>

    public string Role { get; set; } = "User";
}

