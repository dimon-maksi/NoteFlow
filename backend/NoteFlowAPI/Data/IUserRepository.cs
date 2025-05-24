using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Interfaces;

/// <summary>
/// Interface defining operations for accessing and manipulating conspect data.
/// </summary>
public interface IConspectRepository
{
    /// <summary>
    /// Retrieves all conspects.
    /// </summary>
    /// <returns>A list of all conspects.</returns>
    Task<List<Conspect>> GetAllAsync();

    /// <summary>
    /// Retrieves conspects based on query parameters.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering and sorting.</param>
    /// <returns>A filtered and sorted list of conspects.</returns>
    Task<List<Conspect>> GetAsync(ConspectQueryParams queryParams);

    /// <summary>
    /// Retrieves a conspect by its ID.
    /// </summary>
    /// <param name="id">The ID of the conspect to retrieve.</param>
    /// <returns>The conspect if found; otherwise, null.</returns>
    Task<Conspect?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new conspect in the data store.
    /// </summary>
    /// <param name="conspect">The conspect to create.</param>
    Task CreateAsync(Conspect conspect);

    /// <summary>
    /// Updates an existing conspect in the data store.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="conspect">The updated conspect data.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the conspect is not found.</exception>
    Task UpdateAsync(string id, Conspect conspect);

    /// <summary>
    /// Deletes a conspect from the data store.
    /// </summary>
    /// <param name="id">The ID of the conspect to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the conspect is not found.</exception>
    Task DeleteAsync(string id);
}
