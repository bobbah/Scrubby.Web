using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlUserService : SqlServiceBase, IUserService
{
    public SqlUserService(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<ScrubbyUser> GetUser(string phpbbUsername)
    {
        const string query = @"
            SELECT
                u.phpbb_username AS PhpBBUsername,
                COALESCE(ck.byond_key, ck.ckey) AS ByondKey,
                ck.ckey AS ByondCKey
            FROM
                scrubby_users u
                INNER JOIN ckey ck ON ck.ckey = u.ckey
            WHERE
                u.phpbb_username = @phpbbUsername;

            SELECT
                r.name AS role
            FROM
                scrubby_users u
                LEFT JOIN scrubby_role_membership rm ON rm.user = u.phpbb_username
                LEFT JOIN scrubby_role r ON r.id = rm.role
            WHERE
                u.phpbb_username = @phpbbUsername;";

        await using var conn = GetConnection();
        using var multi = await conn.QueryMultipleAsync(query, new { phpbbUsername });

        var user = await multi.ReadSingleOrDefaultAsync<ScrubbyUser>();
        if (user != null)
            user.Roles = (await multi.ReadAsync<string>()).ToList();

        return user;
    }

    public async Task<ScrubbyUser> CreateUser(ScrubbyUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<ScrubbyUser> UpdateUser(ScrubbyUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteUser(ScrubbyUser user)
    {
        throw new NotImplementedException();
    }
}