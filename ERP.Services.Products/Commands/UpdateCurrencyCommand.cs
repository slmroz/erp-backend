using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record UpdateCurrencyCommand(
    int Id,
    string BaseCurrency,
    string TargetCurrency,
    decimal Rate) : ICommand;