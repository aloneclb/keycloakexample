using KeycloakExample.Services;
using System.Text.Json;

namespace KeycloakExample.Filters;

public class RolesFilter : IEndpointFilter
{
    private readonly string[] _roles;

    public RolesFilter(params string[] roles)
    {
        _roles = roles;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        // Kimlik doğrulama kontrolü
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            return Results.Unauthorized();
        }

        // Token'dan resource_access claim'ini al
        // Keycloak kullanıcıya ait client üzerindeki role'leri bu claim'de tutar.
        var resourceAccessClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "resource_access");
        if (resourceAccessClaim is null)
        {
            return Results.Unauthorized();
        }

        var optionsManager = httpContext.RequestServices.GetService<OptionsManager>();
        if (optionsManager is null)
        {
            return Results.Forbid();
        }

        // Token'daki JSON stringini parse et
        var resourceAccess = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(resourceAccessClaim.Value);

        // Client'a özel rolleri bul (örneğin, "my-client")
        if (resourceAccess != null && resourceAccess.TryGetValue(optionsManager.GetClientName, out var clientRoles))
        {
            var roles = clientRoles.GetProperty("roles").EnumerateArray().Select(r => r.GetString());

            // Rolleri kontrol et
            if (_roles.Any(role => roles.Contains(role)))
            {
                return await next(context);
            }
        }

        return Results.Unauthorized();
    }
}

//public class RolesFilter : IEndpointFilter
//{
//    private readonly string[] _roles;

//    public RolesFilter(params string[] roles)
//    {
//        _roles = roles;
//    }

//    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//    {
//        var httpContext = context.HttpContext;

//        // Kimlik doğrulama kontrolü
//        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
//        {
//            return Results.Unauthorized();
//        }
//        // Rol kontrolü
//        //IEnumerable<System.Security.Claims.ClaimsIdentity> a = httpContext.User.Identities;

//        //var b = a.ToList()[0].Claims.ToList();

//        var hasRole = _roles.Any(role => httpContext.User.IsInRole(role));
//        if (!hasRole)
//        {
//            return Results.Forbid();
//        }

//        return await next(context);
//    }
//}