using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WildShape_Sheets_API.Models;
using WildShape_Sheets_API.Services;




var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy => {
                          policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.Configure<WildshapeSheetsDBSettings>(
//    builder.Configuration.GetSection("WildshapeSheetsDB"));



builder.Services.AddSingleton<AppSettings>();
builder.Services.AddSingleton<DataBaseService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<PlayerCharacterService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<HashService>();

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var appSettings = new AppSettings(configuration);

builder.Services.Configure<WildshapeSheetsDBSettings>(configuration.GetSection("WildshapeSheetsDB"));

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
//if (app.Environment.IsDevelopment()) {
//    //app.UseSwagger();
//    //app.UseSwaggerUI();
//}





app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
