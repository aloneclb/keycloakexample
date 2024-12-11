using System.Text.Json.Serialization;

namespace KeycloakExample.Models;

public class KeyCloakModels
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
