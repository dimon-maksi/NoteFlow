using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NoteFlowAPI.Data;
using System.Text;
using MongoDB.Driver;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Repository;
using NoteFlowAPI.Services;

DotEnv.Load();
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// MongoDB setup
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<MongoDbService>().Database);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT_ISSUER"],
            ValidAudience = configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_SECRET"]))
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IConspect, ConspectRepository>();
builder.Services.AddScoped<ConspectServices>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
/*app.UseHttpsRedirection();*/

app.MapGet("/", () =>
    {
        return "Hello World!";
    })
    .WithName("NoteFlow");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();