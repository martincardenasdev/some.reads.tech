using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using some.reads.tech.Features.Authors;
using some.reads.tech.Features.Books;
using some.reads.tech.Features.Users;
using some.reads.tech.Helpers;
using some.reads.tech.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetService<IConfiguration>();
    
    var connectionString = (configuration ?? throw new InvalidOperationException("No connection string found")).GetConnectionString("DefaultConnection");
    
    return new NpgsqlConnectionFactory(connectionString);
});

builder.Services.AddHttpClient<OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org/");
});

builder.Services.AddMemoryCache();

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

app.Run();