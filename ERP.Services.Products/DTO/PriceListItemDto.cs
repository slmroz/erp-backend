namespace ERP.Services.Products.DTO;
public class PriceListItemDto
{
    public int Id { get; set; }
    public int PriceListId { get; set; }
    public string PriceListName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? RemovedAt { get; set; }
    public int? RemovedBy { get; set; }
}
