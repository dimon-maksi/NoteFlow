using MongoDB.Driver;
using NoteFlowAPI.Helpers.QueryParam;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Repository;

/// <summary>
/// MongoDB implementation of the conspect repository.
/// </summary>
public class ConspectRepository : IConspectRepository
{
    private readonly IMongoCollection<Conspect> _conspectCollection;
    private readonly ILogger<ConspectRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the ConspectRepository class.
    /// </summary>
    /// <param name="database">The MongoDB database.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public ConspectRepository(IMongoDatabase database, ILogger<ConspectRepository> logger)
    {
        _conspectCollection = database.GetCollection<Conspect>("conspect");
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all conspects.
    /// </summary>
    /// <returns>A list of all conspects.</returns>
    public async Task<List<Conspect>> GetAllAsync()
    {
        try
        {
            return await _conspectCollection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all conspects");
            throw;
        }
    }

    /// <summary>
    /// Retrieves conspects based on query parameters.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering and sorting.</param>
    /// <returns>A filtered and sorted list of conspects.</returns>
    public async Task<List<Conspect>> GetAsync(ConspectQueryParams queryParams)
    {
        try
        {
            var builder = Builders<Conspect>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTitle))
            {
                filter &= builder.Regex(
                    x => x.Title,
                    new MongoDB.Bson.BsonRegularExpression(queryParams.SearchTitle, "i")
                );
            }

            var sortField = queryParams.SortBy ?? "title";
            var sortDirection =
                queryParams.SortDirection?.ToLower() == "desc"
                    ? Builders<Conspect>.Sort.Descending(sortField)
                    : Builders<Conspect>.Sort.Ascending(sortField);

            return await _conspectCollection.Find(filter).Sort(sortDirection).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conspects with query parameters");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a conspect by its ID.
    /// </summary>
    /// <param name="id">The ID of the conspect to retrieve.</param>
    /// <returns>The conspect if found; otherwise, null.</returns>
    public async Task<Conspect?> GetByIdAsync(string id)
    {
        try
        {
            return await _conspectCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conspect with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new conspect in the data store.
    /// </summary>
    /// <param name="conspect">The conspect to create.</param>
    public async Task CreateAsync(Conspect conspect)
    {
        try
        {
            await _conspectCollection.InsertOneAsync(conspect);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conspect");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing conspect in the data store.
    /// </summary>
    /// <param name="id">The ID of the conspect to update.</param>
    /// <param name="conspect">The updated conspect data.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the conspect is not found.</exception>
    public async Task UpdateAsync(string id, Conspect conspect)
    {
        try
        {
            var result = await _conspectCollection.ReplaceOneAsync(c => c.Id == id, conspect);
            if (result.ModifiedCount == 0)
            {
                throw new KeyNotFoundException($"Conspect with ID {id} not found.");
            }
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conspect with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a conspect from the data store.
    /// </summary>
    /// <param name="id">The ID of the conspect to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the conspect is not found.</exception>
    public async Task DeleteAsync(string id)
    {
        try
        {
            var result = await _conspectCollection.DeleteOneAsync(c => c.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"Conspect with ID {id} not found.");
            }
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conspect with ID: {Id}", id);
            throw;
        }
    }
}
