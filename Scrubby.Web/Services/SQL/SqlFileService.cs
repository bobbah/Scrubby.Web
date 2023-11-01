using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlFileService : SqlServiceBase, IFileService
{
    public SqlFileService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<LogMessage>> GetMessages(FileMessagePostModel model)
    {
        await using var conn = GetConnection();
        var query = new StringBuilder(@"
                SELECT
                    m.timestamp,
                    m.type,
                    m.message,
                    m.origin_name AS OriginName,
                    m.relative_index AS RelativeIndex,
                    m.x,
                    m.y,
                    m.z
                FROM
                    round_file rf
                    INNER JOIN log_message m ON m.parent_file = rf.id
                WHERE
                    rf.round = @RoundID
                    AND rf.name = ANY(@Files)");

        if (model.CKeys != null && model.CKeys.Any())
            query.AppendLine(@" AND m.message ~* ANY(@ckeys)");

        query.AppendLine(" ORDER BY rf.name ASC, m.relative_index ASC");

        return await conn.QueryAsync<LogMessage, SSVec, LogMessage>(query.ToString(), (message, vec) =>
        {
            if (message.Timestamp.HasValue)
                message.Timestamp = message.Timestamp.Value.ToUniversalTime();
            message.Origin = vec;
            return message;
        }, model, splitOn: "x");
    }

    public async Task<ScrubbyFile> GetFile(int id)
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
                    (EXISTS (SELECT 1 FROM log_message m WHERE m.parent_file = rf.id LIMIT 1)) AS decomposed,
                    rf.stockpile_id AS StockpileId
                FROM
                    round_file rf
                WHERE
                    rf.id = @id";

        await using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<ScrubbyFile>(query, new { id });
    }
}