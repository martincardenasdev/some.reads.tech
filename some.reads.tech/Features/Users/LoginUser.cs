using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using some.reads.tech.Helpers;

namespace some.reads.tech.Features.Users
{
    public static class LoginUser
    {
        public static void AddLoginUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("users/login", Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] UserDto request,
            [FromServices] NpgsqlConnectionFactory connectionFactory,
            [FromServices] TokenProvider tokenProvider
            )
        {
            await using var connection = connectionFactory.Create();

            const string sql = @"SELECT id AS Id, password_hash AS PasswordHash FROM users WHERE username = @Username;";

            var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { request.Username });

            if (user is null) return Results.NotFound(new { message = "User not found" });

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Success)
            {
                return Results.Text(tokenProvider.Create(user));
            }
            else
            {
                return Results.BadRequest(new { message = "Invalid password" });
            }
        }
    }
}
