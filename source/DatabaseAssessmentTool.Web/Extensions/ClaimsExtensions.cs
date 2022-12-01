using DatabaseAssessmentTool.Web.Services;
using System.Security.Claims;

namespace DatabaseAssessmentTool.Web.Extensions;

public static class ClaimsExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(ClaimTypes.Role, KeyConstants.ClaimRoleAdmin);
    }

    public static bool IsLoggedIn(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }
}
