using KeycloakExample.Models;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakExample.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/auth")
            .AllowAnonymous()
            .WithTags("Auth API's");

        group.MapGet("get-admin-token", async (KeycloakService service, CancellationToken ct) =>
        {
            var result = await service.GetAccessTokenAsync(ct);
            if (result.isSuccess)
            {
                return Results.Ok(new
                {
                    AccessToken = result.message,
                });
            }

            return Results.BadRequest(new { Error = result.message });
        }).WithSummary("Admin login");


        group.MapPost("register", async ([FromBody] RegisterDto input, KeycloakService service, CancellationToken ct) =>
        {
            var result = await service.RegisterAsync(input, ct);
            if (result.isSuccess)
            {
                return Results.Created();
            }

            return Results.BadRequest(new { Error = result.message });
        }).WithSummary("User Register");

        return routeBuilder;
    }
}
