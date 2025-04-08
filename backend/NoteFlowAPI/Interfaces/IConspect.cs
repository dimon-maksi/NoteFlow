using NoteFlowAPI.Models;
using NoteFlowAPI.Helpers.QueryParam;

namespace NoteFlowAPI.Interfaces;

public interface IConspect
{
    Task<List<Conspect>> GetAsync(ConspectQueryParams queryParams);
    Task<List<Conspect>> GetAsync();
    Task<Conspect> GetByIdAsync(string id);
    Task CreateAsync(Conspect conspectInterfaces);
    Task UpdateAsync(string id, Conspect conspectInterfaces);
    Task DeleteAsync(string id);
}