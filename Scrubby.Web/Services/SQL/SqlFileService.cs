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
                    rf.stockpile_id AS StockpileId
                FROM
                    round_file rf
                WHERE
                    rf.id = @id";

        await using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<ScrubbyFile>(query, new { id });
    }
}