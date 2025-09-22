using System;
using System.Threading.Tasks;
using Scrubby.Web.Models;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Services.SQL;

public class SqlNewscasterService : INewscasterService
{
    public async Task<NewsCasterModel> GetRound(int round) => throw new NotImplementedException();
}