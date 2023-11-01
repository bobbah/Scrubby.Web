using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlRuntimeService : SqlServiceBase, IRuntimeService
{
    public SqlRuntimeService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<ImprovedRuntime>> GetRuntimesForRound(int roundID)
    {
        const string runtimeQuery = @"
                CREATE TEMP TABLE selected_runtimes AS
                SELECT rt.id
                FROM
                    round r
                    INNER JOIN round_file rf ON rf.round = r.id AND rf.name = 'runtime.txt'
                    INNER JOIN runtime rt ON rt.parent_file = rf.id
                WHERE r.id = @round;

                SELECT
                    rt.id,
                    rt.timestamp,
                    rt.exception,
                    rt.proc_path AS procpath,
                    rt.source_file AS sourcefile,
                    rt.line,
                    src.type_path AS typepath,
                    src.name,
                    src.x,
                    src.y,
                    src.z,
                    srcloc.type_path AS typepath,
                    srcloc.name,
                    srcloc.x,
                    srcloc.y,
                    srcloc.z,
                    usr.type_path AS typepath,
                    usr.name,
                    usr.x,
                    usr.y,
                    usr.z,
                    usrloc.type_path AS typepath,
                    usrloc.name,
                    usrloc.x,
                    usrloc.y,
                    usrloc.z
                FROM
                    selected_runtimes srt
                    INNER JOIN runtime rt ON rt.id = srt.id
                    LEFT JOIN atom_reference src ON src.id = rt.source
                    LEFT JOIN atom_reference srcloc ON srcloc.id = rt.source_location
                    LEFT JOIN atom_reference usr ON usr.id = rt.user
                    LEFT JOIN atom_reference usrloc ON usrloc.id = rt.user_location
                ORDER BY
                    rt.timestamp ASC,
                    rt.id ASC;";
        const string stacktraceQuery = @"
                SELECT
                    rt.id,
                    st.index,
                    st.line
                FROM
                    selected_runtimes rt
                    INNER JOIN stacktrace st ON st.parent = rt.id;";

        await using var conn = GetConnection();
        await conn.OpenAsync(); // manually open connection so we can use temp tables between queries

        // Get runtimes
        var results = new Dictionary<long, ImprovedRuntime>();
        await conn
            .QueryAsync<long, ImprovedRuntime, ImprovedAtomReference, ImprovedAtomReference, ImprovedAtomReference,
                ImprovedAtomReference, ImprovedRuntime>(runtimeQuery,
                (id, runtime, src, srcloc, usr, usrloc) =>
                {
                    if (!results.ContainsKey(id))
                    {
                        runtime.Timestamp = runtime.Timestamp.ToUniversalTime(); // Ensure UTC
                        runtime.Stacktrace = new List<ImprovedStacktrace>();
                        runtime.Source = src;
                        runtime.SourceLocation = srcloc;
                        runtime.User = usr;
                        runtime.UserLocation = usrloc;
                        results[id] = runtime;
                    }

                    return results[id];
                }, new { round = roundID }, splitOn: "timestamp, typepath, typepath, typepath, typepath");

        // Get stacktrace entries to tie to runtimes
        await conn.QueryAsync<long, ImprovedStacktrace, ImprovedStacktrace>(stacktraceQuery,
            (runtimeId, stacktrace) =>
            {
                results[runtimeId].Stacktrace.Add(stacktrace);
                return stacktrace;
            }, splitOn: "index");

        // Ensure that stacktraces are ordered correctly
        var toReturn = results.Select(x => x.Value).ToList();
        foreach (var runtime in toReturn) runtime.Stacktrace.Sort((a, b) => a.Index - b.Index);

        return toReturn;
    }
}