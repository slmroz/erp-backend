namespace ERP.Services.Products.DTO;
public class ProductGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public int ProductCount { get; set; } 
}