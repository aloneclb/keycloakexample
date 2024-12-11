using KeycloakExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<OptionsManager>();
//builder.Services.Configure<IdentityServerOption>(builder.Configuration.GetSection("IdentityServer")); options pattern

builder.Services.AddScoped<KeycloakService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// for singleton service
// OptionsManager.Initialize(builder.Configuration);

app.MapGet("get-admin -token", async (KeycloakService service, CancellationToken ct) =>
{
    var result = await service.GetAccessToken(ct);
    if (result.isSuccess)
    {
        return Results.Ok(new
        {
            AccessToken = result.message,
        });
    }

    return Results.BadRequest(new { Error = result.message });
});

app.Run();
