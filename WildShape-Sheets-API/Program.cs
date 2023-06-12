using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<WildshapeSheetsDBSettings>(
    builder.Configuration.GetSection("WildshapeSheetsDB"));

builder.Services.AddSingleton<UserService>();

builder.Services.AddSingleton<AuthService>();
 

builder.Services.AddAuthentication()
    .AddJwtBearer("JwtBearer", JwtBearerOptions =>
    {
        JwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Shared secret key that no one ever knew")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
