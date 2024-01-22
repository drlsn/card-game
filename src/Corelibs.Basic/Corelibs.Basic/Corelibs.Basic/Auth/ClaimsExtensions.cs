using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using System.Security.Claims;

namespace Corelibs.Basic.Auth
{
    public static class ClaimsExtensions
    {
        public static string GetUserID(this ClaimsPrincipal claims)
        {
            var idClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                return null;

            return idClaim.Value;
        }

        public static async Task<string> GetUserID(
            this IAccessorAsync<ClaimsPrincipal> claimsAccessor)
        {
            var user = await claimsAccessor.Get();
            return user.GetUserID();
        }

        public static async Task<TId> GetUserID<TId>(
            this IAccessorAsync<ClaimsPrincipal> claimsAccessor)
            where TId : EntityId
        {
            var user = await claimsAccessor.Get();
            return (TId) Activator.CreateInstance(typeof(TId), user.GetUserID());
        }

        public static bool IsAdmin(this ClaimsPrincipal claims)
        {
            var claim = claims.FindFirst("extension_Roles");
            if (claim is null)
                return false;

            return claim.Value == "Admin";
        }
    }
}
