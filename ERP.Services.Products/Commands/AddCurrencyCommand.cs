using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public class AddCurrencyCommand(
    string BaseCurrency, string TargetCurrency, decimal Rate) : ICommand
{
    public string BaseCurrency { get; } = BaseCurrency;
    public string TargetCurrency { get; } = TargetCurrency;
    public decimal Rate { get; } = Rate;

    public int Id { get; set; }
}