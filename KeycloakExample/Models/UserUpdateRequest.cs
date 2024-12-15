namespace KeycloakExample.Models;

public sealed record UserUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
}


public sealed record PasswordUpdateRequest
{
    public string? Password { get; set; }
}