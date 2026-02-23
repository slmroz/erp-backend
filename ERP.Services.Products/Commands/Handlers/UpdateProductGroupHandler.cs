using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class UpdateProductGroupHandler : ICommandHandler<UpdateProductGroupCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public UpdateProductGroupHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(UpdateProductGroupCommand command)
    {
        var group = await _dbContext.ProductGroups
            .FirstOrDefaultAsync(g => g.Id == command.Id && g.RemovedAt == null)
            ?? throw new KeyNotFoundException("ProductGroup not found");

        // Walidacja unikalności Name (poza bieżącym)
        if (command.Name != group.Name &&
            await _dbContext.ProductGroups.AnyAsync(g => g.Name == command.Name && g.RemovedAt == null))
            throw new InvalidOperationException("ProductGroup name already exists");

        group.Name = command.Name;
        group.Description = command.Description;
        group.LastUpdatedAt = _clock.Current();

        await _dbContext.SaveChangesAsync();
    }
}
