using KeycloakExample.Models;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakExample.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/role")
            .RequireAuthorization()
            .WithTags("Role API's");

        group.MapGet("/get-all", async (KeycloakService service, CancellationToken ct) =>
        {
            (_, var roles) = await service.GetAllClientRolesAsync(ct);
            return Results.Ok(roles);
        }).WithSummary("Tüm client role'ları verir.");

        group.MapGet("/get-name/{roleName:maxlength(50):required}", async ([FromRoute] string roleName, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            (_, var roles) = await service.GetClientRoleByName(roleName, ct);
            return Results.Ok(roles);
        }).WithSummary("İsme göre bir client role getirir.");

        group.MapPost("/create-role", async ([FromBody] CreateRoleDto input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            (_, var message) = await service.CreateClientRoleAsync(input, ct);
            return Results.Ok(message);
        }).WithSummary("Client role oluşturur.");

        group.MapDelete("/delete-role-by-name/{roleName:maxlength(50):required}", async ([FromRoute] string roleName, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var success = await service.DeleteClientRoleByNameAsync(roleName, ct);
            if (!success)
            {
                return Results.BadRequest();
            }
            return Results.Ok();
        }).WithSummary("Client role siler isme göre.");

        return routeBuilder;
    }
}