using KeycloakExample.Models;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakExample.Endpoints;

public static class UserRoleEndpoints
{
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/role")
            .RequireAuthorization()
            .WithTags("User Role API's");

        group.MapPost("assing/{userId:guid:required}", async ([FromRoute] Guid userId, [FromBody] AssignRoleDto input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var isSuccess = await service.AssignRoleToUserAsync(input with
            {
                UserId = userId,
            }, ct);

            return isSuccess ? Results.Ok() : Results.BadRequest();
        }).WithSummary("Kullanıcıya birden çok client role ata.");

        group.MapDelete("unassign/{userId:guid:required}", async ([FromRoute] Guid userId, [FromBody] AssignRoleDto input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var isSuccess = await service.UnAssignRoleToUserAsync(input with
            {
                UserId = userId,
            }, ct);

            return isSuccess ? Results.Ok() : Results.BadRequest();
        }).WithSummary("Kullanıcıdan birden çok client role kaldır.");

        group.MapGet("user/{userId:guid:required}", async ([FromRoute] Guid userId, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var roles = await service.GetAllUserRolesAsync(userId, ct);

            return roles is not null ? Results.Ok(roles) : Results.BadRequest();
        }).WithSummary("Kullanıcıdan tüm client role'lerini getir.");

        return routeBuilder;
    }
}