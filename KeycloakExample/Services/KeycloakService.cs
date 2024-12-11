using KeycloakExample.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
}


#region Dto's

public class KeyCloakDto
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? Token { get; set; }
        [JsonPropertyName("token_type")]
        public string? Type { get; set; }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }

    public class BadRequestResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorDescription { get; set; }
    }
}


#endregion