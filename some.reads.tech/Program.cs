using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using some.reads.tech.Features.Authors;
using some.reads.tech.Features.Books;
using some.reads.tech.Features.Users;
using some.reads.tech.Helpers;
using some.reads.tech.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetService<IConfiguration>();
    
    var connectionString = (configuration ?? throw new InvalidOperationException("No connection string found")).GetConnectionString("DefaultConnection");
    
    return new NpgsqlConnectionFactory(connectionString);
});

builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddHttpClient<OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org/");
});

builder.Services.AddMemoryCache();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddSearchBooksEndpoints();
app.AddSearchAuthorsEndpoints();
app.AddCreateUserEndpoints();
app.AddLoginUserEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();