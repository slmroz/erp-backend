namespace ERP.Services.Products.DTO;
public class ProductDto
{
    public int Id { get; set; }
    public int ProductGroupId { get; set; }
    public string GroupName { get; set; }
    public string PartNumber { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? OemBrand { get; set; }
    public decimal? ListPrice { get; set; }
    public decimal? WeightKg { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
}
