using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlSuicideService : SqlServiceBase, ISuicideService
{
    public SqlSuicideService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<Suicide>> GetSuicidesForCKey(CKey ckey, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = new StringBuilder(@"
                SELECT
                    s.round AS RoundID,
                    m.timestamp AS Timestamp,
                    (m.timestamp - COALESCE(r.game_start_time, r.starttime)) AS RelativeTime,
                    s.ckey AS CKey,
                    m.origin_name AS OriginName,
                    m.timestamp < (COALESCE(r.game_start_time, r.starttime) + INTERVAL '10 minutes') AS RoundStart,
                    rp.role IS NOT NULL AS Antagonist,
                    m.x,
                    m.y,
                    m.z
                FROM
                    suicide s
                    INNER JOIN log_message m ON s.evidence = m.id
                    INNER JOIN round r ON r.id = s.round
                    LEFT JOIN round_player rp ON rp.round = s.round AND rp.ckey = s.ckey AND s.ic_name = rp.name
                WHERE
                    s.ckey = @ckey");
        if (startDate.HasValue && endDate.HasValue)
            query.AppendLine(" AND m.timestamp BETWEEN @startDate AND @endDate");
        else if (startDate.HasValue)
            query.AppendLine(" AND m.timestamp >= @startDate");
        else if (endDate.HasValue)
            query.AppendLine(" AND m.timestamp <= @endDate");
        await using var conn = GetConnection();
        return (await conn.QueryAsync<Suicide, SSVec, Suicide>(query.ToString(), (suicide, vec) =>
        {
            suicide.Origin = vec;
            return suicide;
        }, new { ckey, startDate, endDate }, splitOn: "x")).ToList();
    }
}