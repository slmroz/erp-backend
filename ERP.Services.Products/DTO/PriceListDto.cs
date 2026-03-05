namespace ERP.Services.Products.DTO;
public class PriceListDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int? LastUpdatedBy { get; set; }
    public DateTime? RemovedAt { get; set; }
    public int? RemovedBy { get; set; }

    public int CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public int ItemCount { get; set; }  // Licznik pozycji cenowych
}