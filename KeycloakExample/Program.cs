using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("users", builder =>
    {
        //builder.RequireRealmRoles() eðer realm role kullanýrsak.
        builder.RequireResourceRoles("UserGetAll", "UserCreate", "UserUpdate", "UserDelete"); // biz client role kullanýyoruz.
        // gelen user users policy ile iþaretlenen endpoint'te bu izinlerden birini alsa yeterli.
    });

    // birden fazla ekleyebiliriz.
    //options.AddPolicy("users-policy-2", builder =>
    //{
    //    //builder.RequireRealmRoles() eðer realm role kullanmak istersek.
    //    builder.RequireResourceRoles("UserGetAll", "UserCreate"); // biz client role kullanýyoruz.
    //});
}).AddKeycloakAuthorization(builder.Configuration);

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
app.MapUserRoleEndpoints();

app.Run();