namespace KeycloakExample.Services;

public class KeycloakService(OptionsManager options) // , IOptions<IdentityServerOption> optionsPattern
{
    public IdentityServerOption IdentityServer => options.GetIdentityServer();

    public async Task<string> GetAccessToken(CancellationToken ct)
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

        await client.PostAsync(endpoint, new FormUrlEncodedContent(data), ct);
        return "";
    }
}
