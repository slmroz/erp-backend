using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record UpdatePriceListCommand(
    int Id,
    string Name,
    string? Description,
    int LastUpdatedBy) : ICommand;