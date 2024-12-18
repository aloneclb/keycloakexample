using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Keycloak authentication
// Keycloak authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "http://localhost:8080/realms/example_realm"; // Keycloak realm URL'si
    options.ClientId = "mvc_with_web_login_client"; // Keycloak client ID
    options.ClientSecret = "JFnToPErLDIsnF61jXN3VN9jVe7R4mwY"; // Keycloak client secret
    options.ResponseType = "code";
    options.SaveTokens = true; // ID token ve access token'ý saklamak
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.RequireHttpsMetadata = false; // Geliþtirme ortamýnda HTTPS kullanmayabilirsiniz
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