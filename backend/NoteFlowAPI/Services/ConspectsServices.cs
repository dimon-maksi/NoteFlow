using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Services;

public class ConspectsServices
{
    private readonly ConspectInterfaces _conspectRepository;

    public ConspectsServices(ConspectInterfaces conspectRepository)
    {
        _conspectRepository = conspectRepository;
    }

    public async Task<List<Conspects>> GetAsync() =>
        await _conspectRepository.GetAsync();
    
    public async Task<Conspects> GetByIdAsync(string id) =>
        await _conspectRepository.GetByIdAsync(id);
    
    public async Task CreateAsync(Conspects conspects) =>
        await _conspectRepository.CreateAsync(conspects);
    
    public async Task UpdateAsync(string id, Conspects conspects) =>
        await _conspectRepository.UpdateAsync(id, conspects);
    
    public async Task DeleteAsync(string id) =>
        await _conspectRepository.DeleteAsync(id);
}