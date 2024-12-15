using KeycloakExample.Filters;
using KeycloakExample.Models;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Mvc;

//using Microsoft.AspNetCore.Authorization;

namespace KeycloakExample.Endpoints;

//[Authorize(Roles = "")] minimal api değil ise bu şekilde kullanılabilir.
public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/users")
            .RequireAuthorization()
            //.RequireAuthorization("users") // istersem group'a policy atayabilirim. Program.cs'de tanımladığım
            //.AddEndpointFilter(new RolesFilter("DeleteUser", "UpdateUser"));
            .WithTags("User API's");

        group.MapGet("/get-all", async (KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetAllUsersAsync(ct);
            return Results.Ok(users);
        })
            .RequireAuthorization("users")
            .WithSummary("Tüm user'ları verir.");

        group.MapGet("/get-by-username", async (string username, KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetUserByUsername(username, ct);
            return Results.Ok(users);
        }).WithSummary("Username'e göre kullanıcı ara.");

        group.MapGet("/get-by-email", async (string email, KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetUserByEmail(email, ct);
            return Results.Ok(users);
        })
            .AddEndpointFilter(new RolesFilter("FindByEmailRole")) // ! endpoint filter example.
            .WithSummary("Email'e göre kullanıcı ara.");

        group.MapGet("/get-by-id/{id:guid:required}", async ([FromRoute] Guid id, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            (_, var user) = await service.GetUserByIdAsync(id, ct);
            return Results.Ok(user);
        }).WithSummary("Id'ye göre kullanıcı ara.");

        group.MapPut("/update/{id:guid:required}", async ([FromRoute] Guid id, [FromBody] UserUpdateRequest input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            (_, var user) = await service.UpdateUserAsync(id, input, ct);
            return Results.Ok(user);
        }).WithSummary("Kullanıcının bilgilerini güncelle.");

        group.MapDelete("/delete/{id:guid:required}", async ([FromRoute] Guid id, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var success = await service.DeleteUserAsync(id, ct);
            return Results.Ok();
        })
            .RequireAuthorization("DeleteUser")
            .WithSummary("Kullanıcıyı sil.");

        group.MapPut("/update/{id:guid:required}/password", async ([FromRoute] Guid id, [FromBody] PasswordUpdateRequest input, [FromServices] KeycloakService service, CancellationToken ct) =>
        {
            var success = await service.ResetPasswordAsync(id, input.Password!, ct);
            return Results.Ok();
        }).WithSummary("Kullanıcının şifresini sıfırla.");
        return routeBuilder;
    }
}