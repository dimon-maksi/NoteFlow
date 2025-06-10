namespace NoteFlowAPI.Services;

using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Models;

/// <summary>
/// Interface defining operations for the conspect service.
/// </summary>
public interface IConspectService
{
    /// <summary>
    /// Retrieves all conspects accessible to the specified user.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering and sorting.</param>
    /// <param name="userId">The ID of the user making the request, or null for anonymous users.</param>
    /// <returns>A list of accessible conspects.</returns>
    Task<List<Conspect>> GetAccessibleConspectsAsync(
        ConspectQueryParams queryParams,
        string? userId
    );

    /// <summary>
    /// Retrieves a conspect by its ID.
    /// </summary>
    /// <param name="id">The ID of the conspect to retrieve.</param>
    /// <returns>The conspect if found.</returns>
    Task<Conspect> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new conspect.
    /// </summary>
    /// <param name="conspect">The conspect to create.</param>
    Task CreateAsync(Conspect conspect);

    /// <summary>
    /// Updates an existing conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="conspect">The updated conspect data.</param>
    Task UpdateAsync(string id, Conspect conspect);

    /// <summary>
    /// Deletes a conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to delete.</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Determines if a user can access a specific conspect.
    /// </summary>
    /// <param name="conspect">The conspect to check access for.</param>
    /// <param name="userId">The ID of the user to check, or null for anonymous users.</param>
    /// <returns>True if the user can access the conspect; otherwise, false.</returns>
    bool CanUserAccessConspect(Conspect conspect, string? userId);
}

