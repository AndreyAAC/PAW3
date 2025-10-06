using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PAW3.Data.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }

    public DateTime? LastUpdated { get; set; }

    public int? ProductId { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? ModifiedBy { get; set; }

    [JsonIgnore] public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
