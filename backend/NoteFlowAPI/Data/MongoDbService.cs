using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace NoteFlowAPI.Data;

public class MongoDbService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbService> _logger;

    public MongoDbService(IConfiguration configuration, ILogger<MongoDbService> logger)
    {
        _logger = logger;
        try
        {
            var connectionString = configuration["MongoDB:ConnectionString"] 
                                   ?? configuration["MONGO_CONNECTION_STRING"];
            var databaseName = configuration["MongoDB:DatabaseName"] 
                               ?? configuration["MONGO_DATABASE_NAME"];

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            _client = new MongoClient(settings);
            _database = _client.GetDatabase(databaseName);
            
            _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}")
                .Wait();

            _logger.LogInformation("Successfully connected to MongoDB");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MongoDB");
            throw;
        }
    }

    public IMongoDatabase Database => _database;
}