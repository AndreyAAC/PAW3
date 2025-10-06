using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PAW3.Data.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? InventoryId { get; set; }

    public int? SupplierId { get; set; }

    public string? Description { get; set; }

    public decimal? Rating { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? LastModified { get; set; }

    public string? ModifiedBy { get; set; }

    [JsonIgnore] public virtual Category? Category { get; set; }

    [JsonIgnore] public virtual Inventory? Inventory { get; set; }

    [JsonIgnore] public virtual Supplier? Supplier { get; set; }
}
