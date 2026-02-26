using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class RemoveCurrencyHandler : ICommandHandler<RemoveCurrencyCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public RemoveCurrencyHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(RemoveCurrencyCommand command)
    {
        var currency = await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.RemovedAt == null)
            ?? throw new KeyNotFoundException("Currency rate not found");

        // Walidacja użycia (np. w Opportunities/Products)
        // if (await _dbContext.Opportunities.AnyAsync(o => o.CurrencyId == command.Id))
        //     throw new InvalidOperationException("Currency used in active opportunities");

        currency.RemovedAt = _clock.Current();
        await _dbContext.SaveChangesAsync();
    }
}
