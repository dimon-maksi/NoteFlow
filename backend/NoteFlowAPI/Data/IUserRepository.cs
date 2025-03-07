namespace NoteFlowAPI.Data;

using NoteFlowAPI.Models;
using System.Threading.Tasks;

/// <summary>
/// Interface defining operations for accessing and manipulating user data.
/// </summary>

public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>

    Task<User?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Creates a new user in the data store.
    /// </summary>
    /// <param name="user">The user to create.</param>

    Task CreateUserAsync(User user);
}

