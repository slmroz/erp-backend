using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;

public class AddPriceListItemCommand : ICommand
{
    public AddPriceListItemCommand(
        int PriceListId,
        int ProductId,
        decimal Price,
        int? CreatedBy)
    {
        this.PriceListId = PriceListId;
        this.ProductId = ProductId;
        this.Price = Price;
        this.CreatedBy = CreatedBy;
    }

    public int PriceListId { get; }
    public int ProductId { get; }
    public decimal Price { get; }
    public int? CreatedBy { get; }
    public int Id { get; set;  }
}

