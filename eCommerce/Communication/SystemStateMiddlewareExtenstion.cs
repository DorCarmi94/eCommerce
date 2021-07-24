using Microsoft.AspNetCore.Builder;

namespace eCommerce.Communication
{
    public static class SystemStateMiddlewareExtenstion
    {
        public static IApplicationBuilder UseSystemStateValidator(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SystemStateMiddleware>();
        }
    }
}