namespace KeycloakExample.Models;

public sealed record RegisterDto
{
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ImageUrl { get; set; }
}

public sealed record LoginDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}