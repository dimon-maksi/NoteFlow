namespace NoteFlowAPI.Data;

using NoteFlowAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
    }

    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task CreateUserAsync(User user) =>
        await _users.InsertOneAsync(user);
}

