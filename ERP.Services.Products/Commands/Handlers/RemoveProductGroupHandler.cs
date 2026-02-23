using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;

internal sealed class RemoveProductGroupHandler : ICommandHandler<RemoveProductGroupCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public RemoveProductGroupHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(RemoveProductGroupCommand command)
    {
        var group = await _dbContext.ProductGroups
            .FirstOrDefaultAsync(g => g.Id == command.Id && g.RemovedAt == null)
            ?? throw new KeyNotFoundException("ProductGroup not found");

        // Walidacja: brak aktywnych produktów w grupie
        if (await _dbContext.Products.AnyAsync(p => p.ProductGroupId == command.Id && p.RemovedAt == null))
            throw new InvalidOperationException("Cannot remove group with active products");

        group.RemovedAt = _clock.Current();
        await _dbContext.SaveChangesAsync();
    }
}
