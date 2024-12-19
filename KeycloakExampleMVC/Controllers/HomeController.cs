using KeycloakExampleMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KeycloakExampleMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var userName = User.Identity?.Name; // Kullanýcý adý
            var userClaims = User.Claims; // Kullanýcýya ait tüm talepler
            return View();
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }

        public async Task<IActionResult> Logout()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token"); // id_token'ý al
            Console.WriteLine(idToken);
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            ////return Redirect("http://localhost:8080/realms/example_realm/protocol/openid-connect/logout?id_token_hint=2d74aad9-f4f6-44e0-b1d3-d1bdd4f58d59&post_logout_redirect_uri=http%3A%2F%2Flocalhost%3A5013%2Fsignout-callback-oidc&state=CfDJ8EyINPt8s0lElKjYVxgiZuYKIQkHLGpF0Cn-C2tonM9Z5JM6IJsyQHeDSIjn5GqO4-4Piok0M0HJV9XVhrYPmFtM3yeiqKj5xmbVHxDKqtu7X5R90sh2hiXZAJdyS_VA2hEaehkbt_B7VKr1pCBKV2Q&x-client-SKU=ID_NET9_0&x-client-ver=8.2.1.0");

            //if (!string.IsNullOrEmpty(idToken))
            //{
            //    // nameidentifier "2d74aad9-f4f6-44e0-b1d3-d1bdd4f58d59"
            //    properties.Parameters.Add("id_token_hint", idToken); // id_token_hint parametresi ekle
            //    properties.Parameters.Add("client", idToken); // id_token_hint parametresi ekle
            //}

            return SignOut(
                properties,
                OpenIdConnectDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme);
        }




        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "deneme")]
        public IActionResult DenemeRole()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}