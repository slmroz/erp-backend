using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;

public class AddPriceListCommand : ICommand
{
    public AddPriceListCommand(
    string Name,
    string? Description,
    int CreatedBy,
    int CurrencyId,
    decimal? DiscountPercentage,
    bool FillItems)
    {
        this.Name = Name;
        this.Description = Description;
        this.CreatedBy = CreatedBy;
        this.CurrencyId = CurrencyId;
        this.DiscountPercentage = DiscountPercentage;
        this.FillItems = FillItems;
    }

    public string Name { get; }
    public string? Description { get; }
    public int CreatedBy { get; }
    public int CurrencyId { get; }
    public decimal? DiscountPercentage { get; }
    public bool FillItems { get; }
    public int? Id { get; set; }
}

;
