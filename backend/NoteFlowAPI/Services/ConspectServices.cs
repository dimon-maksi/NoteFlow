using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Services;

/// <summary>
/// Service that implements conspect operations.
/// </summary>
public class ConspectService : IConspectService
{
    private readonly IConspectRepository _conspectRepository;
    private readonly ILogger<ConspectService> _logger;

    /// <summary>
    /// Initializes a new instance of the ConspectService class.
    /// </summary>
    /// <param name="conspectRepository">Repository for conspect data access.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public ConspectService(IConspectRepository conspectRepository, ILogger<ConspectService> logger)
    {
        _conspectRepository = conspectRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a conspect by its ID.
    /// </summary>
    /// <param name="id">The ID of the conspect to retrieve.</param>
    /// <returns>The conspect if found.</returns>
    public async Task<Conspect> GetByIdAsync(string id)
    {
        try
        {
            return await _conspectRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conspect with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new conspect.
    /// </summary>
    /// <param name="conspect">The conspect to create.</param>
    public async Task CreateAsync(Conspect conspect)
    {
        try
        {
            await _conspectRepository.CreateAsync(conspect);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conspect");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="conspect">The updated conspect data.</param>
    public async Task UpdateAsync(string id, Conspect conspect)
    {
        try
        {
            await _conspectRepository.UpdateAsync(id, conspect);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conspect with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a conspect.
    /// </summary>
    /// <param name="id">The ID of the conspect to delete.</param>
    public async Task DeleteAsync(string id)
    {
        try
        {
            await _conspectRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conspect with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all conspects accessible to the specified user.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering and sorting.</param>
    /// <param name="userId">The ID of the user making the request, or null for anonymous users.</param>
    /// <returns>A list of accessible conspects.</returns>
    public async Task<List<Conspect>> GetAccessibleConspectsAsync(
        ConspectQueryParams queryParams,
        string? userId
    )
    {
        try
        {
            var allConspects = await _conspectRepository.GetAsync(queryParams);

            if (string.IsNullOrEmpty(userId))
            {
                return allConspects.Where(c => !c.IsPrivate).ToList();
            }
            else
            {
                return allConspects
                    .Where(c =>
                        !c.IsPrivate
                        || c.UserId == userId
                        || (c.AllowedUserIds != null && c.AllowedUserIds.Contains(userId))
                    )
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accessible conspects");
            throw;
        }
    }

    /// <summary>
    /// Determines if a user can access a specific conspect.
    /// </summary>
    /// <param name="conspect">The conspect to check access for.</param>
    /// <param name="userId">The ID of the user to check, or null for anonymous users.</param>
    /// <returns>True if the user can access the conspect; otherwise, false.</returns>
    public bool CanUserAccessConspect(Conspect conspect, string? userId)
    {
        if (!conspect.IsPrivate)
            return true;

        if (string.IsNullOrEmpty(userId))
            return false;

        return conspect.UserId == userId
            || (conspect.AllowedUserIds != null && conspect.AllowedUserIds.Contains(userId));
    }
}
