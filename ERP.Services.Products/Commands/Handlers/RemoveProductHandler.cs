using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class RemoveProductHandler : ICommandHandler<RemoveProductCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public RemoveProductHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(RemoveProductCommand command)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == command.Id && p.RemovedAt == null)
            ?? throw new KeyNotFoundException("Product not found");

        //// Walidacja: brak aktywnych Opportunities/Orders z tym produktem
        //if (await _dbContext.Opportunities.AnyAsync(o => o.ProductId == command.Id && o.RemovedAt == null))
        //    throw new InvalidOperationException("Cannot remove product used in active opportunities");

        product.RemovedAt = _clock.Current();
        await _dbContext.SaveChangesAsync();
    }
}
