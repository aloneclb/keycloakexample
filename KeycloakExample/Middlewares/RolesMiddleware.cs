namespace KeycloakExample.Middlewares;

public class RolesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _roles;

    public RolesMiddleware(RequestDelegate next, string[] roles)
    {
        _next = next;
        _roles = roles;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Kullanıcının kimliği doğrulanmış mı kontrol et
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // Kullanıcının rolleri kontrol et
        var hasRole = _roles.Any(role => context.User.IsInRole(role));
        if (!hasRole)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        // Kullanıcı gerekli role sahipse bir sonraki middleware'e devam et
        await _next(context);
    }
}

public static class RolesMiddlewareExtensions
{
    public static IApplicationBuilder UseRoles(this IApplicationBuilder app, params string[] roles)
    {
        return app.UseMiddleware<RolesMiddleware>(roles);
    }
}