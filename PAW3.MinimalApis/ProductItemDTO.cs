public class ProductItemDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public ProductItemDTO() { }
    public ProductItemDTO(Product productItem) =>
    (Id, Name, IsComplete) = (productItem.Id, productItem.Name, productItem.IsComplete);
}