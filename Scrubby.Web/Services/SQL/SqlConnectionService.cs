using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlConnectionService : SqlServiceBase, IConnectionService
{
    public SqlConnectionService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<ServerRoundStatistic>> GetConnectionStatsForCKey(CKey ckey, DateTime startDate)
    {
        const string query = @"
                SELECT
                    s.display AS server,
                    r.starttime::date AS date,
                    COUNT(DISTINCT r.id),
                    ROUND((EXTRACT(EPOCH FROM SUM(c.disconnect_time - c.connect_time)) / (3600.0))::numeric, 2) AS hours
                FROM
                    round r
                    INNER JOIN connection c ON c.round = r.id
                    LEFT JOIN server s ON r.server = s.id
                WHERE
                    c.ckey = @ckey
                    AND c.connect_time >= @startDate
                GROUP BY
                    s.display,
                    r.starttime::date";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<ServerRoundStatistic>(query, new { ckey = ckey.Cleaned, startDate })).ToList();
    }

    public async Task<List<ServerConnection>> GetConnectionsForRound(int round, IEnumerable<string> ckeys = null)
    {
        var query = new StringBuilder(@"
                SELECT
                    c.round AS RoundID,
                    COALESCE(ck.byond_key, ck.ckey) AS ckey,
                    c.connect_time AS ConnectTime,
                    c.disconnect_time AS DisconnectTime,
                    c.byond_version AS ByondVersion,
                    s.display AS server
                FROM
                    connection c
                    LEFT JOIN ckey ck ON ck.ckey = c.ckey
                    INNER JOIN round r ON r.id = c.round
                    INNER JOIN server s ON s.id = r.server
                WHERE
                    c.round = @round");

        if (ckeys != null)
            query.AppendLine(@" AND c.ckey = ANY(@ckeys)");

        await using var conn = GetConnection();
        return (await conn.QueryAsync<ServerConnection>(query.ToString(), new { round, ckeys })).ToList();
    }
}