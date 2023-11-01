using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlAnnouncementService : SqlServiceBase, IAnnouncementService
{
    public SqlAnnouncementService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<ScrubbyAnnouncement>> GetAnnouncements(bool onlyActive = true)
    {
        const string activeQuery = @"
                SELECT a.*
                FROM announcements a
                WHERE
                    a.active
                    AND (a.start IS NULL OR a.start < CURRENT_TIMESTAMP)
                    AND (a.end IS NULL OR a.end > CURRENT_TIMESTAMP)";
        const string allQuery = @"SELECT a.* FROM announcements a";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<ScrubbyAnnouncement>(onlyActive ? activeQuery : allQuery)).ToList();
    }

    public async Task<ScrubbyAnnouncement> CreateAnnouncement(ScrubbyAnnouncement announcement)
    {
        const string query = @"
                INSERT INTO announcements (""start"", ""end"", message, active) 
                VALUES (@start, @end, @message, @active) 
                RETURNING id, ""start"", ""end"", message, active";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<ScrubbyAnnouncement>(query, announcement)).FirstOrDefault();
    }

    public async Task<ScrubbyAnnouncement> UpdateAnnouncement(ScrubbyAnnouncement announcement)
    {
        const string query = @"
                UPDATE announcements
                SET
                    ""start"" = @start,
                    ""end"" = @end,
                    message = @message,
                    active = @active
                WHERE id = @id
                RETURNING id, ""start"", ""end"", message, active";
        await using var conn = GetConnection();
        return (await conn.QueryAsync<ScrubbyAnnouncement>(query, announcement)).FirstOrDefault();
    }

    public async Task<bool> DeleteAnnouncement(ScrubbyAnnouncement announcement)
    {
        const string query = @"DELETE FROM announcements WHERE id = @id";
        await using var conn = GetConnection();
        return await conn.ExecuteAsync(query, announcement) == 1;
    }
}