using KeycloakExample.Models;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakExample.Endpoints;

public static class UserRoleEndpoints
{
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/role")
            .AllowAnonymous()
            .WithTags("User Role API's");

        group.MapPost("assing/{userId:guid:required}", async ([FromRoute] Guid userId, [FromBody] AssignRoleDto input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var isSuccess = await service.AssignRoleToUserAsync(input with
            {
                UserId = userId,
            }, ct);

            return isSuccess ? Results.Ok() : Results.BadRequest();
        });

        return routeBuilder;
    }
}