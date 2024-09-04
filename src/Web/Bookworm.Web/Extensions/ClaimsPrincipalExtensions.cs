namespace Bookworm.Web.Extensions
{
    using System.Security.Claims;

    using static System.Security.Claims.ClaimTypes;
    using static Bookworm.Common.Constants.GlobalConstants;

    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
            => user.FindFirstValue(NameIdentifier);

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdministratorRoleName);
    }
}
