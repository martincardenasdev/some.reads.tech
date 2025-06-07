using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using some.reads.tech.Database;
using some.reads.tech.Features.Authors;
using some.reads.tech.Features.Books;
using some.reads.tech.Features.Books.Get_Book;
using some.reads.tech.Features.Bookshelves.Add_to_bookshelf;
using some.reads.tech.Features.Users;
using some.reads.tech.Helpers;
using some.reads.tech.Services;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "some.reads.tech API", Version = "v1" });
    c.TagActionsBy(api => [api.RelativePath.Split('/')[0].ToUpper()]);
});

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetService<IConfiguration>();
    
    var connectionString = (configuration ?? throw new InvalidOperationException("No connection string found")).GetConnectionString("DefaultConnection");
    
    return new NpgsqlConnectionFactory(connectionString);
});

builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<DatabaseInitializer>();

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

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

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
app.AddGetBookEndpoints();
app.AddAddToBookshelfEndpoints();

app.UseAuthentication();
app.UseAuthorization();

if (args.Contains("--migrate"))
{
    var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.Initialize();
}

app.Run();