using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Services;

public class ConspectServices
{
    private readonly IConspect _conspectRepository;

    public ConspectServices(IConspect conspectRepository)
    {
        _conspectRepository = conspectRepository;
    }

    public async Task<List<Conspect>> GetAsync() =>
        await _conspectRepository.GetAsync();
    
    public async Task<Conspect> GetByIdAsync(string id) =>
        await _conspectRepository.GetByIdAsync(id);
    
    public async Task CreateAsync(Conspect conspect) =>
        await _conspectRepository.CreateAsync(conspect);
    
    public async Task UpdateAsync(string id, Conspect conspect) =>
        await _conspectRepository.UpdateAsync(id, conspect);
    
    public async Task DeleteAsync(string id) =>
        await _conspectRepository.DeleteAsync(id);
    
    public async Task<List<Conspect>> GetAsync(ConspectQueryParams queryParams) =>
        await _conspectRepository.GetAsync(queryParams);
}