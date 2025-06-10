using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NoteFlowAPI.Data;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Middleware;
using NoteFlowAPI.Repository;
using NoteFlowAPI.Services;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }));
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

// Database services
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IMongoDatabase>(provider =>
    provider.GetRequiredService<MongoDbService>().Database
);

// Authentication services
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenBlacklistService>();

// Repository services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IConspectRepository, ConspectRepository>();

// Business logic services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConspectService, ConspectService>();

builder.Services.AddControllers();

var jwtSecret =
    configuration["JWT_SECRET"]
    ?? throw new ArgumentNullException("JWT_SECRET is missing from env");

var jwtIssuer = configuration["JWT_ISSUER"];
var jwtAudience = configuration["JWT_AUDIENCE"];

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
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

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
