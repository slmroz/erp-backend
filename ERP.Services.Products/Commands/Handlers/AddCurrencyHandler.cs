using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class AddCurrencyHandler : ICommandHandler<AddCurrencyCommand>
{
    private readonly ErpContext _dbContext; 

    public AddCurrencyHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task HandleAsync(AddCurrencyCommand command)
    {
        // Walidacja unikalności BaseCurrency+TargetCurrency
        var key = $"{command.BaseCurrency}|{command.TargetCurrency}";
        if (await _dbContext.Currencies.AnyAsync(c =>
            c.BaseCurrency == command.BaseCurrency &&
            c.TargetCurrency == command.TargetCurrency &&
            c.RemovedAt == null))
            throw new InvalidOperationException($"Currency pair {key} already exists");

        var currency = new Currency  // EF model
        {
            BaseCurrency = command.BaseCurrency.ToUpper(),
            TargetCurrency = command.TargetCurrency.ToUpper(),
            Rate = command.Rate,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Currencies.Add(currency);
        await _dbContext.SaveChangesAsync();
        command.Id = currency.Id;
    }
}