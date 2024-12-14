using Keycloak.AuthServices.Authentication;
using KeycloakExample.Endpoints;
using KeycloakExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecuritySchema = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** yourt JWT Bearer token on textbox below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecuritySchema.Reference.Id, jwtSecuritySchema);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecuritySchema, Array.Empty<string>() } });
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<OptionsManager>();
//builder.Services.Configure<IdentityServerOption>(builder.Configuration.GetSection("IdentityServer")); options pattern

builder.Services.AddScoped<KeycloakService>();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// for singleton service
// OptionsManager.Initialize(builder.Configuration);

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapRoleEndpoints();

app.Run();