using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Services.IEntitiesServices {
    public interface IIdentityService
    {
        Task<ClaimsPrincipal> CheckUserAsync(string email,string password);

        Task<ClaimsPrincipal> CheckUserAsync(string email, string username, string password);

        Task<IdentityResult> RegisterAsync(string email, string uasernma, string password);
    }
}
