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

        group.MapGet("/get-by-username", async (string username, KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetUserByUsername(username, ct);
            return Results.Ok(users);
        }).WithSummary("Username'e göre kullanıcı ara.");

        group.MapGet("/get-by-email", async (string email, KeycloakService service, CancellationToken ct) =>
        {
            (_, var users) = await service.GetUserByEmail(email, ct);
            return Results.Ok(users);
        }).WithSummary("Email'e göre kullanıcı ara.");

        group.MapGet("/get-by-id", async (Guid id, KeycloakService service, CancellationToken ct) =>
        {
            (_, var user) = await service.GetUserById(id, ct);
            return Results.Ok(user);
        }).WithSummary("Id'ye göre kullanıcı ara.");

        return routeBuilder;
    }
}