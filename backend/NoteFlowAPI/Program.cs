using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NoteFlowAPI.Data;
using NoteFlowAPI.Services;

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IMongoDatabase>(provider =>
    provider.GetRequiredService<MongoDbService>().Database
);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TokenBlacklistService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var jwtSecret =
    Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new ArgumentNullException("JWT_SECRET is missing from env");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

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
