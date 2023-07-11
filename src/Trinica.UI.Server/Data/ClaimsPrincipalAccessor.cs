using Corelibs.Basic.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Trinica.UI.Server.Data;

public class ClaimsPrincipalAccessor : IAccessorAsync<ClaimsPrincipal>
{
    private readonly AuthenticationStateProvider _auth;

    public ClaimsPrincipalAccessor(AuthenticationStateProvider auth)
    {
        _auth = auth;
    }

    public async Task<ClaimsPrincipal> Get()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        return state.User;
    }
}
