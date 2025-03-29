namespace NoteFlowAPI.DTOs;

/// <summary>
/// Data Transfer Object for user login requests.
/// </summary>

public class LoginRequestDto
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

