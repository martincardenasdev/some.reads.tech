using Npgsql;

namespace some.reads.tech.Shared
{
    public static class PostgresExceptionHandler
    {
        public static IResult Handle(Exception ex)
        {
            return ex switch
            {
                PostgresException { SqlState: "23505" } => Results.Conflict(new { message = "Conflict: Duplicate entry" }),
                PostgresException { SqlState: "23503" } => Results.NotFound(new { message = "Not Found: Foreign key violation" }),
                PostgresException { SqlState: "23502" } => Results.BadRequest(new { message = "Bad Request: Null value in non-nullable column" }),
                _ => Results.BadRequest(new { message = "An error occurred while processing the request" })
            };
        }
    }
}
