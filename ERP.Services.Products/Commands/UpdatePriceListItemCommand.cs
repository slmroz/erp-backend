using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;

namespace ERP.Services.Products.Commands;
public record UpdatePriceListItemCommand(
    int Id,
    int PriceListId,
    int ProductId,
    decimal Price,
    int LastUpdatedBy) : ICommand;

