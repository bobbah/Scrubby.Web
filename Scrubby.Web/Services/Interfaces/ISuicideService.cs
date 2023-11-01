using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Services.Interfaces;

public interface ISuicideService
{
    Task<List<Suicide>> GetSuicidesForCKey(CKey ckey, DateTime? startDate = null,
        DateTime? endDate = null);
}