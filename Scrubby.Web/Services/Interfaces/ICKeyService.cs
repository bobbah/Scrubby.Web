using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Services.Interfaces;

public interface ICKeyService
{
    Task<List<NameCountRecord>> GetNamesForCKeyAsync(CKey ckey);
    Task<List<ServerStatistic>> GetServerCountForCKeyAsync(CKey ckey);
    Task<string> GetByondKeyAsync(CKey ckey);
    Task<SqlCKey> GetKeyDetailsAsync(string ckey, bool requestIfNotFound = false);
}