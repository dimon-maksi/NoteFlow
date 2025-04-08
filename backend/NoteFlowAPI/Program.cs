using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NoteFlowAPI.Data;
using NoteFlowAPI.Middleware;
using NoteFlowAPI.Services;
using NoteFlowAPI.Interfaces;
using NoteFlowAPI.Repository;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }));
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IMongoDatabase>(provider =>
    provider.GetRequiredService<MongoDbService>().Database
);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenBlacklistService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConspect, ConspectRepository>();
builder.Services.AddScoped<ConspectServices>();

builder.Services.AddControllers();

var jwtSecret =
    configuration["JWT_SECRET"]
    ?? throw new ArgumentNullException("JWT_SECRET is missing from env");

var jwtIssuer = configuration["JWT_ISSUER"];
var jwtAudience = configuration["JWT_AUDIENCE"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
