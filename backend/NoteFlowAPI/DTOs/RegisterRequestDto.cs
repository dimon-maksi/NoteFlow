namespace NoteFlowAPI.DTOs;

/// <summary>
/// Data Transfer Object for user registration requests.
/// </summary>

public class RegisterRequestDto
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>

    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>

    public string Password { get; set; } = string.Empty;
}

