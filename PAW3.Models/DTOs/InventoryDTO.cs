using System.Text.Json.Serialization;

namespace PAW3.Models.DTOs;

public class InventoryDTO
{
    [JsonPropertyName("inventoryId")]
    public int InventoryId { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal? UnitPrice { get; set; }

    [JsonPropertyName("unitsInStock")]
    public int? UnitsInStock { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; set; }

    [JsonPropertyName("modifiedBy")]
    public string? ModifiedBy { get; set; }
}