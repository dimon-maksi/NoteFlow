using NoteFlowAPI.Models;

namespace NoteFlowAPI.Interfaces;

public interface ConspectInterfaces
{
    Task<List<Conspects>> GetAsync();
    Task<Conspects> GetByIdAsync(string id);
    Task CreateAsync(Conspects conspectInterfaces);
    Task UpdateAsync(string id, Conspects conspectInterfaces);
    Task DeleteAsync(string id);
}