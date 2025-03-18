using NoteFlowAPI.Models;
using NoteFlowAPI.Interfaces;
using MongoDB.Driver;

namespace NoteFlowAPI.Repository;

public class ConspectRepository : IConspect
{
    private readonly IMongoCollection<Conspect> _conspectCollection;

    public ConspectRepository(IMongoDatabase database)
    {
        _conspectCollection = database.GetCollection<Conspect>("conspect");
    }
    
    
    public async Task<List<Conspect>> GetAsync()
    {
        try
        {
            return await _conspectCollection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error geting conspect: {ex.Message}");
            throw;
        }
    }

    public async Task<Conspect> GetByIdAsync(string id)
    {
        try
        {
            return await _conspectCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting conspect by id: {ex.Message}");
            throw;
        }
    }

    public async Task CreateAsync(Conspect conspect)
    {
        try
        {
            await _conspectCollection.InsertOneAsync(conspect);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating conspect: {ex.Message}");
            throw;
        }
    }


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