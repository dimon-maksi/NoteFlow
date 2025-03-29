namespace NoteFlowAPI.Data;

using NoteFlowAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

/// <summary>
/// MongoDB implementation of the user repository.
/// </summary>

public class UserRepository : IUserRepository
{
    
    private readonly IMongoCollection<User> _users;
    
    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="database">The MongoDB database.</param>

    public UserRepository(IMongoDatabase database)
    {
      _users = database.GetCollection<User>("Users");
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    
    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

    /// <summary>
    /// Creates a new user in the MongoDB database.
    /// </summary>
    /// <param name="user">The user to create.</param>
    
    public async Task CreateUserAsync(User user) =>
        await _users.InsertOneAsync(user);

    /// <summary>
    /// Updates an existing user in the MongoDB database.
    /// </summary>
    /// <param name="user">The user to update.</param>
    public async Task UpdateUserAsync(User user) =>
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
}

