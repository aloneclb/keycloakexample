using KeycloakExample.Services;

namespace KeycloakExample.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/role")
            .AllowAnonymous()
            .WithTags("Role API's");

        group.MapGet("/get-all", async (KeycloakService service, CancellationToken ct) =>
        {
            (_, var roles) = await service.GetAllClientRolesAsync(ct);
            return Results.Ok(roles);
        }).WithSummary("Tüm client role'ları verir.");



        return routeBuilder;
    }
}
