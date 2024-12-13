using KeycloakExample.Services;

namespace KeycloakExample.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/users")
            .AllowAnonymous()
            .WithTags("Auth API's");

        group.MapGet("/get-all", async (KeycloakService service, CancellationToken ct) =>
        {

        });

        return routeBuilder;
    }
}