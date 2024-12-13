using KeycloakExample.Models;
using System.Text;
using System.Text.Json;

namespace KeycloakExample.Services;

public class KeycloakService(OptionsManager options) // , IOptions<IdentityServerOption> optionsPattern
{
    public IdentityServerOption IdentityServer => options.GetIdentityServer();

    public async Task<(bool isSuccess, string? message)> GetAccessTokenAsync(CancellationToken ct)
    {
        HttpClient client = new HttpClient();
        var endpoint = $"{IdentityServer.HostName}/realms/{IdentityServer.RealmName}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new()
        {
             new("grant_type", "client_credentials"),
             new("client_id", IdentityServer.ClientName),
             new("client_secret", IdentityServer.ClientSecret)
        };

        //var grantType = new KeyValuePair<string, string>("grant_type", "client_credentials");
        //var clientId = new KeyValuePair<string, string>("client_id", IdentityServer.ClientName);
        //var clientSecret = new KeyValuePair<string, string>("client_secret", IdentityServer.ClientSecret);

        var message = await client.PostAsync(endpoint, new FormUrlEncodedContent(data), ct);
        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var badResponse = JsonSerializer.Deserialize<KeyCloakDto.BadRequestResponse>(response);
                return (false, badResponse?.ErrorDescription);
            }

            var errorResponse = JsonSerializer.Deserialize<KeyCloakDto.ErrorResponse>(response);
            return (false, errorResponse?.ErrorDescription);
        }

        var successResponse = JsonSerializer.Deserialize<KeyCloakDto.TokenResponse>(response);
        return (true, successResponse?.Token);
    }

    public async Task<(bool isSuccess, string? message)> RegisterAsync(RegisterDto request, CancellationToken ct)
    {
        var tokenResponse = await GetAccessTokenAsync(ct);
        if (!tokenResponse.isSuccess)
        {
            return (false, tokenResponse.message);
        }

        object data = new
        {
            username = request.Username,
            firstName = request.FirstName,
            email = request.Email,
            enabled = true,
            emailVerified = false,
            credentials = new object[1]
            {
                new {
                    type = "password",
                    temporary = false,
                    value = request.Password
                }
            },
            attributes = new List<object>() // özel attribute'lar
            {
                new {
                    ImageUrl = request.ImageUrl,
                    deneme = ".net client tarafından oluşturuldu"
                }
            }
        };

        var endpoint = $"{IdentityServer.HostName}/admin/realms/{IdentityServer.RealmName}/users";
        var stringData = JsonSerializer.Serialize(data);
        var content = new StringContent(stringData, encoding: Encoding.UTF8, mediaType: "application/json");

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse.message}");
        var message = await client.PostAsync(endpoint, content, ct);

        if (!message.IsSuccessStatusCode)
        {
            var response = await message.Content.ReadAsStringAsync();
            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var badResponse = JsonSerializer.Deserialize<KeyCloakDto.BadRequestResponse>(response);
                return (false, badResponse?.ErrorDescription);
            }

            var errorResponse = JsonSerializer.Deserialize<KeyCloakDto.ErrorResponse>(response);
            return (false, errorResponse?.ErrorDescription);
        }

        return (true, "User başarılı bir şekilde oluşturuldu");
    }

    public async Task<(bool isSuccess, string? message)> LoginAsync(LoginDto request, CancellationToken ct)
    {
        HttpClient client = new HttpClient();
        var endpoint = $"{IdentityServer.HostName}/realms/{IdentityServer.RealmName}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new()
        {
             new("grant_type", "password"),
             new("client_id", IdentityServer.ClientName),
             new("client_secret", IdentityServer.ClientSecret),
             new("username", request.Username!),
             new("password", request.Password!)
        };

        var message = await client.PostAsync(endpoint, new FormUrlEncodedContent(data), ct);
        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var badResponse = JsonSerializer.Deserialize<KeyCloakDto.BadRequestResponse>(response);
                return (false, badResponse?.ErrorDescription);
            }

            var errorResponse = JsonSerializer.Deserialize<KeyCloakDto.ErrorResponse>(response);
            return (false, errorResponse?.ErrorDescription);
        }

        var result = JsonSerializer.Deserialize<KeyCloakDto.TokenResponse>(response);
        if (result?.Token is null)
        {
            return (false, "Parse edilemedi");
        }

        return (true, result?.Token);
    }

    public async Task<(bool isSuccess, List<KeyCloakDto.UserDto>? users)> GetAllUsersAsync(CancellationToken ct)
    {
        var tokenResponse = await GetAccessTokenAsync(ct);
        if (!tokenResponse.isSuccess)
        {
            return (false, null);
        }

        var endpoint = $"{IdentityServer.HostName}/admin/realms/{IdentityServer.RealmName}/users";
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse.message}");
        var message = await client.GetAsync(endpoint, ct);
        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var badResponse = JsonSerializer.Deserialize<KeyCloakDto.BadRequestResponse>(response);
                return (false, null);
            }

            var errorResponse = JsonSerializer.Deserialize<KeyCloakDto.ErrorResponse>(response);
            return (false, null);
        }

        List<KeyCloakDto.UserDto>? users = JsonSerializer.Deserialize<List<KeyCloakDto.UserDto>>(response);
        return (true, users);

    }
}