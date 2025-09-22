using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.CommonRounds;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlRoundService : SqlServiceBase, IRoundService
{
    public SqlRoundService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<ScrubbyRound> GetRound(int id)
    {
        var roundQuery = GetRoundInternal(id);
        var fileQuery = GetFiles(id);
        var processQuery = GetProcesses(id);
        var playersQuery = GetRoundPlayers(id);
        await Task.WhenAll(roundQuery, fileQuery, processQuery, playersQuery);
        var toReturn = roundQuery.Result;

        // If we found no round, do not bother trying to associate related data...
        if (toReturn == null)
            return null;

        // Associate related data
        toReturn.Files = fileQuery.Result.ToList();
        toReturn.Status = processQuery.Result.ToList();
        toReturn.Players = playersQuery.Result.ToList();
        return toReturn;
    }

    public async Task<int> GetNext(int id, bool forward = true, List<string> ckey = null)
    {
        await using var conn = GetConnection();
        var query = new StringBuilder();
        if (ckey != null && ckey.Any())
            query.AppendLine("SELECT id FROM get_rounds_for_players(@ckey, true)");
        else
            query.AppendLine("SELECT r.id FROM round r");
        query.AppendLine(
            $" WHERE id {(forward ? ">" : "<")} @id ORDER BY id {(forward ? "ASC" : "DESC")} LIMIT 1");
        return await conn.QueryFirstOrDefaultAsync<int?>(query.ToString(), new { ckey, id }) ?? -1;
    }

    public async Task<List<CommonRoundModel>> GetCommonRounds(IEnumerable<string> ckeys,
        CommonRoundsOptions options = null)
    {
        const string query = @"
                WITH common_rounds AS (
                    SELECT
                        c.round
                    FROM
                        connection c
                    WHERE
                        c.ckey = ANY(@ckeys)
                        AND (@startingRound IS NULL OR (
                            CASE WHEN @gte THEN c.round >= @startingRound ELSE c.round <= @startingRound END))
                    GROUP BY
                        c.round
                    HAVING
                        COUNT(DISTINCT c.ckey) = @ckeyCount
                    LIMIT @limit
                )
                SELECT
                    r.id AS round,
                    r.starttime AS started,
                    r.endtime AS ended
                FROM
                    common_rounds cr
                    INNER JOIN round r ON r.id = cr.round
                ORDER BY
                    r.id ASC";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<CommonRoundModel>(query, new
        {
            ckeys,
            ckeyCount = ckeys.Count(),
            startingRound = options?.StartingRound,
            gte = options?.GTERound,
            limit = options?.Limit
        })).ToList();
    }

    private async Task<ScrubbyRound> GetRoundInternal(int roundId)
    {
        ScrubbyRound toReturn = null;
        await using var conn = GetConnection();
        const string query = @"
                SELECT
                    r.id,
                    u.url AS BaseURL,
                    r.starttime,
                    r.endtime,
                    r.map,
                    r.map_loading_seconds AS maploadingseconds,
                    r.game_start_duration AS gamestartduration,
                    r.game_start_time AS gamestarttime,
                    s.display AS server,
                    s.internal AS internal,
                    r.origin_master_commit AS master,
                    r.head_commit AS head,
                    tm.pr,
                    tm.commit
                FROM
                    round r
                    INNER JOIN round_url u ON u.round = r.id AND u.active
                    INNER JOIN server s ON s.id = r.server
                    LEFT JOIN round_file rf ON rf.round = r.id AND rf.name = 'runtime.txt'
                    LEFT JOIN testmerge tm ON tm.parent_file = rf.id
                WHERE
                    r.id = @roundId";

        await conn.QueryAsync<ScrubbyRound, RoundBuildInfo, TestMerge, ScrubbyRound>(query,
            (round, version, testmerge) =>
            {
                toReturn ??= round;
                if (version == null)
                    return round;

                toReturn.VersionInfo ??= version;
                toReturn.VersionInfo.TestMerges ??= new List<TestMerge>();
                if (testmerge != null)
                    toReturn.VersionInfo.TestMerges.Add(testmerge);

                return round;
            }, new { roundId }, splitOn: "master,pr");

        // Found no record of this round
        if (toReturn == null)
            return null;

        // Set UTC
        toReturn.StartTime = toReturn.StartTime.ToUniversalTime();
        if (toReturn.EndTime.HasValue)
            toReturn.EndTime = toReturn.EndTime.Value.ToUniversalTime();
        if (toReturn.GameStartTime.HasValue)
            toReturn.GameStartTime = toReturn.GameStartTime.Value.ToUniversalTime();

        return toReturn;
    }

    private async Task<IEnumerable<ScrubbyFile>> GetFiles(int round)
    {
        const string query = @"
                SELECT
                    rf.id,
                    rf.name,
                    rf.round,
                    rf.size,
                    rf.uploaded,
                    rf.processed,
                    rf.failed,
                    rf.stockpile_id AS StockpileId
                FROM
                    round r
                    INNER JOIN round_file rf ON rf.round = r.id
                WHERE
                    r.id = @round";
        await using var conn = GetConnection();
        return await conn.QueryAsync<ScrubbyFile>(query, new { round });
    }

    private async Task<IEnumerable<ProcessStatus>> GetProcesses(int round)
    {
        const string query = @"
                SELECT
                    rp.process,
                    rp.message,
                    rp.timestamp,
                    rp.status
                FROM
                    round r
                    INNER JOIN round_process rp ON rp.round = r.id
                WHERE
                    r.id = @round";
        await using var conn = GetConnection();
        return await conn.QueryAsync<ProcessStatus>(query, new { round });
    }

    private async Task<IEnumerable<Round.RoundPlayer>> GetRoundPlayers(int round)
    {
        const string query = @"
                SELECT
                    COALESCE(c.byond_key, c.ckey) AS ckey,
                    rp.name,
                    LOWER(rp.job) AS job,
                    COALESCE(rp.role, 'none') AS role,
                    CASE WHEN (rp.jointime BETWEEN r.game_start_time AND (r.game_start_time + make_interval(0, 0, 0, 0, 0, 0, r.game_start_duration))) THEN 'ROUNDSTART' WHEN rp.jointime IS NOT NULL THEN 'LATEJOIN' ELSE NULL END AS jointime,
                    rp.round AS roundid,
                    rp.ckey AS CleanKey
                FROM
                    round r
                    INNER JOIN round_player rp ON rp.round = r.id
                    LEFT JOIN ckey c ON c.ckey = rp.ckey
                WHERE
                    r.id = @round";
        await using var conn = GetConnection();
        return await conn.QueryAsync<Round.RoundPlayer>(query, new { round });
    }
}