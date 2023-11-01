using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlMaintenanceService : SqlServiceBase, IMaintenanceService
{
    public SqlMaintenanceService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task ReparseRoundsAsync(IEnumerable<int> rounds, bool redownloadFiles)
    {
        await using var conn = GetConnection();
        foreach (var round in rounds)
        {
            if (redownloadFiles)
                await conn.ExecuteAsync(@"
                    CALL scrubby_reparse_round(@round);
                    DELETE FROM round_file WHERE round = @round;
                    DELETE FROM round_process WHERE round = @round AND process <> 'Download';
                    UPDATE round_process SET status = 'Waiting' WHERE round = @round AND process = 'Download';", new { round });
            else
                await conn.ExecuteAsync("CALL scrubby_reparse_round(@round)", new { round });
        }
    }

    public async Task ReparseRoundsWithFileAsync(string fileName)
    {
        await using var conn = GetConnection();

        // Get rounds first
        var rounds = await conn.QueryAsync<int>("SELECT DISTINCT round FROM round_file WHERE name = @fileName", new { fileName });
        
        // Then process those rounds
        foreach (var round in rounds)
        {
            await conn.ExecuteAsync("CALL scrubby_reparse_round(@round)", new { round });
        }
    }

    public async Task<IEnumerable<Guid>> GetObjectsForArchiveDiscovery(int round)
    {
        await using var conn = GetConnection();
        return await conn.QueryAsync<Guid>(@"
            WITH files AS (SELECT rf.name FROM round_file rf WHERE rf.round = @round GROUP BY rf.name ORDER BY COUNT(*) DESC LIMIT 1)
            SELECT rf.stockpile_id FROM files INNER JOIN round_file rf ON rf.round = @round AND rf.name = files.name", new { round });
    }
}