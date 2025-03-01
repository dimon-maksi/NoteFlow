namespace NoteFlowAPI.Data;

using NoteFlowAPI.Models;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task CreateUserAsync(User user);
}

