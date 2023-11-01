using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlScrubbyService : SqlServiceBase, IScrubbyService
{
    public SqlScrubbyService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<BasicStatsModel> GetBasicStats()
    {
        var toReturn = await GetStatsModelBase();
        toReturn.LatestRounds = (await GetLatest()).ToList();
        return toReturn;
    }

    private async Task<IEnumerable<LatestRound>> GetLatest()
    {
        await using var conn = GetConnection();
        return await conn.QueryAsync<LatestRound>(
            "SELECT r.server, r.starttime AS Started, r.round AS Id FROM latest_rounds r");
    }

    private async Task<BasicStatsModel> GetStatsModelBase()
    {
        const string query = @"
                SELECT
                    (SELECT COUNT(*) FROM round) AS RoundCount,
                    (SELECT COUNT(*) FROM round_file) AS FileCount,
                    pg_database_size('scrubby') AS DatabaseSize";
        await using var conn = GetConnection();
        return await conn.QueryFirstAsync<BasicStatsModel>(query);
    }
}