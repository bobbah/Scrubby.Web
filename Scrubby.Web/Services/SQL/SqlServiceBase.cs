using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Scrubby.Web.Services.SQL;

public abstract class SqlServiceBase
{
    private readonly string _connectionString;

    public SqlServiceBase(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("mn3");
    }

    protected NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}