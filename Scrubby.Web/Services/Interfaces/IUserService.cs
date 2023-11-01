using System.Threading.Tasks;
using Scrubby.Web.Models.Data;

namespace Scrubby.Web.Services.Interfaces;

public interface IUserService
{
    public Task<ScrubbyUser> GetUser(string phpbbUsername);
    public Task<ScrubbyUser> CreateUser(ScrubbyUser user);
    public Task<ScrubbyUser> UpdateUser(ScrubbyUser user);
    public Task<bool> DeleteUser(ScrubbyUser user);
}