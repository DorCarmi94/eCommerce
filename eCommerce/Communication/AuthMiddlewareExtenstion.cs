using Microsoft.AspNetCore.Builder;

namespace eCommerce.Communication
{
    public static class AuthMiddlewareExtenstion
    {
        public static IApplicationBuilder UseAuth(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}