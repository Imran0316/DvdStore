using Microsoft.AspNetCore.Http;

namespace DvdStore.Models
{
    public static class AuthorizationHelper
    {
        public static bool IsAdmin(HttpContext context)
        {
            var userId = context.Session.GetInt32("UserId");
            var userRole = context.Session.GetString("UserRole");
            return userId != null && userRole == "Admin";
        }

        public static bool IsLoggedIn(HttpContext context)
        {
            return context.Session.GetInt32("UserId") != null;
        }
    }
}