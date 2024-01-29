using Corelibs.Basic.Repository;
using System.Security.Claims;

namespace Trinica.Api.Authorization
{
    public class CurrentUserAccessor : IAccessorAsync<ClaimsPrincipal>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

       
        Task<ClaimsPrincipal> IAccessorAsync<ClaimsPrincipal>.Get() =>
            Task.FromResult(_httpContextAccessor.HttpContext.User);
    }
}
