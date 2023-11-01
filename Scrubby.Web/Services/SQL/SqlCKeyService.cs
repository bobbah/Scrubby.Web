using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlCKeyService : SqlServiceBase, ICKeyService
{
    private readonly BYONDDataService _byond;

    public SqlCKeyService(IConfiguration configuration, BYONDDataService byond) : base(configuration)
    {
        _byond = byond;
    }

    public async Task<List<NameCountRecord>> GetNamesForCKeyAsync(CKey ckey)
    {
        const string query = @"
                SELECT
                    rp.name,
                    COUNT(*)
                FROM
                    round_player rp
                WHERE
                    rp.ckey = @ckey
                GROUP BY
                    rp.name
                ORDER BY
                    count desc";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<NameCountRecord>(query, new { ckey = ckey.Cleaned })).ToList();
    }

    public async Task<List<ServerStatistic>> GetServerCountForCKeyAsync(CKey ckey)
    {
        const string query = @"
                WITH rounds AS (
                    SELECT DISTINCT
                        c.round,
                        c.ckey
                    FROM
                        connection c
                    WHERE
                        c.ckey = @ckey
                )
                SELECT
                    s.display AS server,
                    COUNT(*) FILTER (WHERE rp.id IS NOT NULL) AS played,
                    COUNT(*) FILTER (WHERE rp.id IS NULL) AS connected,
                    COALESCE(EXTRACT(EPOCH FROM SUM((SELECT SUM(c.disconnect_time - c.connect_time) FROM connection c WHERE c.round = r_init.round AND c.ckey = r_init.ckey)) FILTER (WHERE rp.id IS NOT NULL)) * 1000, 0) AS playedmillisec,
                    COALESCE(EXTRACT(EPOCH FROM SUM((SELECT SUM(c.disconnect_time - c.connect_time) FROM connection c WHERE c.round = r_init.round AND c.ckey = r_init.ckey)) FILTER (WHERE rp.id IS NULL)) * 1000, 0) AS connectedmillisec 
                FROM
                    rounds r_init
                    INNER JOIN round r ON r.id = r_init.round
                    LEFT JOIN server s ON s.id = r.server
                    LEFT JOIN round_player rp ON rp.round = r_init.round AND rp.ckey = r_init.ckey
                GROUP BY
                    s.display";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<ServerStatistic>(query, new { ckey = ckey.Cleaned })).ToList();
    }

    public async Task<string> GetByondKeyAsync(CKey ckey)
    {
        const string query = @"SELECT ck.byond_key FROM ckey ck WHERE ck.ckey = @ckey";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<string>(query, new { ckey = ckey.Cleaned })).FirstOrDefault();
    }

    public async Task<SqlCKey> GetKeyDetailsAsync(string ckey, bool requestIfNotFound = false)
    {
        const string query = @"
                SELECT
                    ck.ckey,
                    ck.byond_key AS byondkey,
                    ck.joined_byond AS joinedbyond,
                    ck.user_not_found AS usernotfound,
                    ck.user_inactive AS userinactive
                FROM
                    ckey ck
                WHERE
                    ck.ckey = @ckey";
        await using var conn = GetConnection();
        var result = (await conn.QueryAsync<SqlCKey>(query, new { ckey })).FirstOrDefault();

        if (result != null || !requestIfNotFound) return result;

        // Try to request the data from BYOND if necessary when not found
        result = new SqlCKey
        {
            CKey = SqlCKey.SanitizeKey(ckey)
        };
        try
        {
            var data = await _byond.GetUserData(ckey, CancellationToken.None);
            if (data != null)
            {
                result.JoinedBYOND = data.Joined;
                result.ByondKey = data.Key;
            }
        }
        catch (BYONDUserNotFoundException)
        {
            result.UserNotFound = true;
        }
        catch (BYONDUserInactiveException)
        {
            result.UserInactive = true;
        }

        return result;
    }
}