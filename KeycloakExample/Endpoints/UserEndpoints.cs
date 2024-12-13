using KeycloakExample.Services;

namespace KeycloakExample.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/users")
            .RequireAuthorization()
            .WithTags("User API's");

        group.MapGet("/get-all", async (KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetAllUsersAsync(ct);
            return Results.Ok(users);
        }).WithSummary("Tüm user'ları verir.");

        return routeBuilder;
    }
}