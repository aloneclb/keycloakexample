namespace KeycloakExample.Models;

public record UserUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
}


public record PasswordUpdateRequest
{
    public string? Password { get; set; }
}