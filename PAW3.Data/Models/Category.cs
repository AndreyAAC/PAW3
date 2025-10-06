using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PAW3.Data.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime? LastModified { get; set; }

    public string? ModifiedBy { get; set; }

    [JsonIgnore] public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
