using Corelibs.Basic.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Trinica.Entities.Users;

namespace Trinica.UI.Common.Auth;

public static class AuthenticationStateProviderExtensions
{
    public static async Task<bool> IsSignedIn(this AuthenticationStateProvider auth)
    {
        var state = await auth.GetAuthenticationStateAsync();
        return state.User.Identity.IsAuthenticated;
    }

    public static async Task<string> GetLabel(this AuthenticationStateProvider auth)
    {
        var state = await auth.GetAuthenticationStateAsync();
        if (!state.User.Identity.IsAuthenticated)
            return "Sign In";

        return "Sign Out";
    }

    public static async Task SignAction(this AuthenticationStateProvider auth, NavigationManager nav)
    {
        var state = await auth.GetAuthenticationStateAsync();
        if (!state.User.Identity.IsAuthenticated)
            nav.NavigateTo($"MicrosoftIdentity/Account/SignIn", forceLoad: true);
        else
            nav.NavigateTo($"MicrosoftIdentity/Account/SignOut", forceLoad: true);
    }

    public static async Task<string> GetUserId(this AuthenticationStateProvider auth)
    {
        var state = await auth.GetAuthenticationStateAsync();
        return state.User.GetUserID();
    }

    public static async Task<bool> IsAdmin(this AuthenticationStateProvider auth)
    {
        var state = await auth.GetAuthenticationStateAsync();
        return state.User.IsAdmin();
    }
}
