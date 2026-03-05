using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record RemovePriceListCommand(int Id, int RemovedBy) : ICommand;