using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record RemoveCurrencyCommand(int Id) : ICommand;
