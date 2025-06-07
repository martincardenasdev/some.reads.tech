using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using some.reads.tech.Helpers;
using some.reads.tech.Shared;

namespace some.reads.tech.Features.Users
{
    public static class CreateUser
    {
        private static readonly User User = new();
        public static void AddCreateUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("users/create", Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] UserDto request,
            NpgsqlConnectionFactory connectionFactory,
            IValidator<UserDto> validator
            )
        {
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.IsValid is false)
                return Results.BadRequest(validationResult.Errors);

            var hashedPassword = new PasswordHasher<User>().HashPassword(User, request.Password);

            User.Username = request.Username;
            User.PasswordHash = hashedPassword;

            await using var connection = connectionFactory.Create();

            const string sql =
                @"INSERT INTO users (username, password_hash) 
                  VALUES (@Username, @PasswordHash) 
                  RETURNING id;";

            try
            {
                var createdUser = await connection.ExecuteScalarAsync<Guid>(sql, new { User.Username, User.PasswordHash });

                return Results.Ok(new
                {
                    message = "User created successfully",
                    userId = createdUser,
                });
            }
            catch (Exception ex)
            {
                return PostgresExceptionHandler.Handle(ex);
            }
        }
    }
}
