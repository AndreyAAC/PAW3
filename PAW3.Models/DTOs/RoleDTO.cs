using System.Text.Json.Serialization;

namespace PAW3.Models.DTOs;

public class RoleDTO
{
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }
}