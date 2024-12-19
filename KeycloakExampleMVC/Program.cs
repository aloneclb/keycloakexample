using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Keycloak authentication MVC için cookie li

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        // cookie authenticationda 
        // eðer duruma gelen dataya vs. göre ekstra claimler eklemek istersek
        OnSigningIn = context =>
        {
            var principal = context.Principal;
            var identity = (System.Security.Claims.ClaimsIdentity)principal.Identity;

            // client-roles verildi custom mapper adýna ondan burada gelicek.
            var roles = identity.FindAll("client-roles").ToList();
            //if (roles.Any())
            //{
            //    foreach (var role in roles)
            //    {
            //        identity.AddClaim(new System.Security.Claims.Claim("roles", role.Value));
            //    }
            //}

            return Task.CompletedTask;
        }
    };
})
.AddOpenIdConnect(options =>
{
    options.Authority = "http://localhost:8080/realms/example_realm"; // realm URL'si
    options.ClientId = "mvc_with_web_login_client"; // client ID
    options.ClientSecret = "JFnToPErLDIsnF61jXN3VN9jVe7R4mwY"; // client secret
    options.ResponseType = "code";
    options.SaveTokens = true; // id token ve access tokený saklamak
    options.RequireHttpsMetadata = false; // ssl kullanmak

    // claimlere eklenilmek istenilen scope'larý gir
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("basic");
    options.Scope.Add("acr");
    options.Scope.Add("address");
    options.Scope.Add("imageUrl");
    options.Scope.Add("microprofile-jwt");
    options.Scope.Add("offline_access");
    options.Scope.Add("organization");

    options.Scope.Add("client-roles"); // roller için kendi mapperim

    // token kontrol
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "http://localhost:8080/realms/example_realm",
        ValidateAudience = true,
        ValidAudience = "mvc_with_web_login_client"
    };
});

// Authorization (Optional)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.RequireAuthenticatedUser(); // Varsayýlan yetkilendirme politikasý
    });
}).AddKeycloakAuthorization(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();