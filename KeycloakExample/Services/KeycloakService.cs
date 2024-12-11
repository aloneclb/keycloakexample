using KeycloakExample.Models;
using System.Text.Json;

namespace KeycloakExample.Services;

public class KeycloakService(OptionsManager options) // , IOptions<IdentityServerOption> optionsPattern
{
    public IdentityServerOption IdentityServer => options.GetIdentityServer();

    public async Task<(bool isSuccess, string? message)> GetAccessToken(CancellationToken ct)
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
                var badResponse = JsonSerializer.Deserialize<KeyCloakModels.BadRequestResponse>(response);
                return (false, badResponse?.ErrorDescription);
            }

            var errorResponse = JsonSerializer.Deserialize<KeyCloakModels.ErrorResponse>(response);
            return (false, errorResponse?.ErrorDescription);
        }

        var successResponse = JsonSerializer.Deserialize<KeyCloakModels.TokenResponse>(response);
        return (true, successResponse?.Token);
    }
}
