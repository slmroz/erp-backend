using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record RemovePriceListItemCommand(int Id, int RemovedBy) : ICommand;


