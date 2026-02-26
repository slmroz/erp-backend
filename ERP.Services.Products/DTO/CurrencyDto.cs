namespace ERP.Services.Products.DTO;
public class CurrencyDto
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; }
    public string TargetCurrency { get; set; }
    public decimal Rate { get; set; }  // DECIMAL(18,6)
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
}