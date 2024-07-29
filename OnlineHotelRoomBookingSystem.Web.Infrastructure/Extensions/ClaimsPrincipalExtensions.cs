namespace OnlineHotelRoomBookingSystem.Web.Infrastructure.Extensions;

using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;
using System.Security.Claims;
public static class ClaimsPrincipalExtensions
{
    public static string? GetId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(AdminRoleName);
    }
}
