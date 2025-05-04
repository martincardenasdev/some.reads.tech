using Npgsql;

namespace some.reads.tech.Helpers;

public class NpgsqlConnectionFactory(string connectionString)
{
    public NpgsqlConnection Create()
    {
        return new NpgsqlConnection(connectionString);
    }
}