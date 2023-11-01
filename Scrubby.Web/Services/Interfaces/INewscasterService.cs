using System.Threading.Tasks;
using Scrubby.Web.Models;

namespace Scrubby.Web.Services.Interfaces;

public interface INewscasterService
{
    public Task<NewsCasterModel> GetRound(int round);
}