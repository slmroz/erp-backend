using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class UpdateCurrencyHandler : ICommandHandler<UpdateCurrencyCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public UpdateCurrencyHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(UpdateCurrencyCommand command)
    {
        var currency = await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.RemovedAt == null)
            ?? throw new KeyNotFoundException("Currency rate not found");

        // Walidacja unikalności (poza bieżącym)
        var key = $"{command.BaseCurrency}|{command.TargetCurrency}";
        if (command.Id != currency.Id &&
            await _dbContext.Currencies.AnyAsync(c =>
                c.BaseCurrency == command.BaseCurrency &&
                c.TargetCurrency == command.TargetCurrency &&
                c.RemovedAt == null))
            throw new InvalidOperationException($"Currency pair {key} already exists");

        currency.BaseCurrency = command.BaseCurrency.ToUpper();
        currency.TargetCurrency = command.TargetCurrency.ToUpper();
        currency.Rate = command.Rate;
        currency.LastUpdatedAt = _clock.Current();

        await _dbContext.SaveChangesAsync();
    }
}
