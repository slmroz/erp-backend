using ERP.Model.Model;
using ERP.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Commands.Handlers;
public sealed class RemoveCustomerHandler : ICommandHandler<RemoveCustomerCommand>
{
    private readonly ErpContext _db;

    public RemoveCustomerHandler(ErpContext db)
    {
        _db = db;
    }

    public async Task HandleAsync(RemoveCustomerCommand command)
    {
        var customer = await _db.Customers
            .Where(c => c.Id == command.Id && c.RemovedAt == null)
            .FirstOrDefaultAsync();

        if (customer == null)
            throw new KeyNotFoundException("Customer not found");

        customer.RemovedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}