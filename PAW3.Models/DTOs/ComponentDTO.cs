using System.Text.Json.Serialization;

namespace PAW3.Models.DTOs;

public class ComponentDTO
{
    [JsonPropertyName("id")]
    public decimal Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}