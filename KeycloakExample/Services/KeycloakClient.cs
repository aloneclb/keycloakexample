using System.Text.Json.Serialization;

namespace KeycloakExample.Services;

// todo: KeycloakService içerisindeki http isteklerini bu client'e taşı
public sealed class KeycloakClient : BaseHttpClient
{
    protected override Task AttachTokenToRequestAsync(HttpRequestMessage request, CancellationToken stoppingToken = default)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask<HttpClient> CreateHttpClientAsync()
    {

        throw new NotImplementedException();
    }

    protected override ValueTask<Uri> CreateUrlAsync(string action)
    {
        throw new NotImplementedException();
    }

    protected override Task RemoveTokenFromCacheAsync()
    {
        throw new NotImplementedException();
    }

    public void GetAccessTokenAsync()
    {

    }
}


#region Dto's

public class KeyCloakDto
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("createdTimestamp")]
        public long CreatedTimeStamp { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("attributes")]
        public Attribute Attributes { get; set; } = new();

        public class Attribute
        {
            public List<string>? ImageUrl { get; set; }
        }
    }

    public class RoleDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("composite")]
        public bool Composite { get; set; }

        [JsonPropertyName("clientRole")]
        public bool IsClientRole { get; set; }

        [JsonPropertyName("containerId")]
        public Guid ContainerId { get; set; }
    }


    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? Token { get; set; }
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
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