namespace Bookworm.Web.Extensions
{
    using System.Security.Claims;

    using static System.Security.Claims.ClaimTypes;

    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
            => user.FindFirstValue(NameIdentifier);
    }
}
