using NoteFlowAPI.Models;
using NoteFlowAPI.Interfaces;
using MongoDB.Driver;

namespace NoteFlowAPI.Repository;

public class ConspectRepository : ConspectInterfaces
{
    private readonly IMongoCollection<Conspects> _conspectCollection;

    public ConspectRepository(IMongoDatabase database)
    {
        _conspectCollection = database.GetCollection<Conspects>("conspects");
    }
    
    
    public async Task<List<Conspects>> GetAsync()
    {
        try
        {
            return await _conspectCollection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error geting conspects: {ex.Message}");
            throw;
        }
    }

    public async Task<Conspects> GetByIdAsync(string id)
    {
        try
        {
            return await _conspectCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting conspects by id: {ex.Message}");
            throw;
        }
    }

    public async Task CreateAsync(Conspects conspects)
    {
        try
        {
            await _conspectCollection.InsertOneAsync(conspects);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating conspect: {ex.Message}");
            throw;
        }
    }


    public async Task UpdateAsync(string id, Conspects conspects)
    {
        try
        {
            var result = await _conspectCollection.ReplaceOneAsync(c => c.Id == id, conspects);
            if (result.ModifiedCount == 0)
            {
                throw new KeyNotFoundException($"Conspect with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating conspect: {ex.Message}");
            throw;
        }
    }

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
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting book: {ex.Message}");
            throw;
        }
    }
}