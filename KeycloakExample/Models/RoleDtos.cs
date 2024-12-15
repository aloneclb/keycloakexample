using System.Text.Json.Serialization;

namespace KeycloakExample.Models;

public sealed record CreateRoleDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public sealed record AssignRoleDto
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public List<Role> Roles { get; set; } = new();

    public sealed record Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}