namespace ERP.Services.Products.DTO;
public class CurrencyRateDto
{
    public string BaseCurrency { get; set; }
    public DateTime Date { get; set; }
    public string TargetCurrency { get; set; }
    public decimal Rate { get; set; }
    public decimal ConvertedAmount { get; set; }
}
