using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlPlayerService : SqlRoundService, IPlayerService
{
    public SqlPlayerService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<PlayerNameStatistic>> SearchForCKey(Regex regex)
    {
        const string query = @"
                SELECT
                    COALESCE(ck.byond_key, ck.ckey) AS CKey,
                    COUNT(*)
                FROM
                    ckey ck
                    LEFT JOIN connection c ON c.ckey = ck.ckey
                WHERE
                    ck.ckey ~* @regex
                GROUP BY
                    COALESCE(ck.byond_key, ck.ckey)
                ORDER BY
                    count DESC";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<PlayerNameStatistic>(query, new { regex = regex.ToString() })).ToList();
    }

    public async Task<List<PlayerNameStatistic>> SearchForICName(Regex regex)
    {
        const string query = @"
                SELECT
                    rp.name AS icname,
                    COALESCE(ck.byond_key, rp.ckey) AS ckey,
                    COUNT(*)
                FROM
                    round_player rp
                    LEFT JOIN ckey ck ON ck.ckey = rp.ckey
                WHERE
                    rp.name ~* @regex
                GROUP BY
                    rp.name,
                    COALESCE(ck.byond_key, rp.ckey)
                ORDER BY
                    count DESC";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<PlayerNameStatistic>(query, new { regex = regex.ToString() })).ToList();
    }

    public async Task<List<RoundReceipt>> GetRoundReceiptsForPlayer(CKey ckey, int startingRound, int limit)
    {
        const string query = @"
                WITH rounds AS (
                    SELECT DISTINCT c.round, c.ckey FROM connection c WHERE c.ckey = @ckey AND c.round < @startingRound ORDER BY c.round DESC LIMIT @limit
                )
                SELECT
                    r.id AS roundid,
                    rp.job,
                    r.starttime AS timestamp,
                    (SELECT SUM(c.disconnect_time - c.connect_time) FROM connection c WHERE c.ckey = r_init.ckey AND c.round = r.id) AS connectedtime,
                    CASE WHEN rp.ckey IS NOT NULL AND rp.jointime BETWEEN r.game_start_time AND (r.game_start_time + make_interval(0, 0, 0, 0, 0, 0, r.game_start_duration)) THEN TRUE ELSE FALSE END AS roundstartplayer,
                    rp.job IS NOT NULL AS playedinround,
                    rp.role IS NOT NULL AS antagonist,
                    rp.name,
                    s.display AS server
                FROM
                    rounds r_init
                    INNER JOIN round r ON r_init.round = r.id 
                    LEFT JOIN server s ON s.id = r.server
                    LEFT JOIN round_player rp ON rp.round = r.id AND rp.ckey = r_init.ckey";

        await using var conn = GetConnection();
        return (await conn.QueryAsync<RoundReceipt>(query, new { ckey = ckey.Cleaned, startingRound, limit })).ToList();
    }

    public async Task<List<RoundReceipt>> GetRoundReceiptsForPlayer(CKey ckey, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        const string query = @"
                WITH rounds AS (
                    SELECT DISTINCT c.round, c.ckey 
                    FROM connection c 
                    WHERE 
                        c.ckey = @ckey AND (
                            (@startDate::date IS NULL AND @endDate::date IS NULL) 
                            OR (@startDate::date IS NULL AND c.connect_time < @endDate::date)
                            OR (@endDate::date IS NULL AND c.connect_time > @startDate::date)
                            OR (c.connect_time BETWEEN @startDate::date AND @endDate::date)
                        ) 
                    ORDER BY c.round DESC
                )
                SELECT
                    r.id AS roundid,
                    rp.job,
                    r.starttime AS timestamp,
                    (SELECT SUM(c.disconnect_time - c.connect_time) FROM connection c WHERE c.ckey = r_init.ckey AND c.round = r.id) AS connectedtime,
                    CASE WHEN rp.ckey IS NOT NULL AND rp.jointime BETWEEN r.game_start_time AND (r.game_start_time + make_interval(0, 0, 0, 0, 0, 0, r.game_start_duration)) THEN TRUE ELSE FALSE END AS roundstartplayer,
                    rp.job IS NOT NULL AS playedinround,
                    rp.role IS NOT NULL AS antagonist,
                    rp.name,
                    s.display AS server
                FROM
                    rounds r_init
                    INNER JOIN round r ON r_init.round = r.id 
                    LEFT JOIN server s ON s.id = r.server
                    LEFT JOIN round_player rp ON rp.round = r.id AND rp.ckey = r_init.ckey";

        await using var conn = GetConnection();
        return (await conn.QueryAsync<RoundReceipt>(query, new { ckey = ckey.Cleaned, startDate, endDate })).ToList();
    }
}