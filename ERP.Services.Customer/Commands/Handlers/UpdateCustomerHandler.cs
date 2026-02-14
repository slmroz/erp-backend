using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Commands.Handlers;
public sealed class UpdateCustomerHandler : ICommandHandler<UpdateCustomerCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _db;

    public UpdateCustomerHandler(ErpContext db, IClock clock)
    {
        _clock = clock;
        _db = db;
    }

    public async Task HandleAsync(UpdateCustomerCommand command)
    {
        var customer = await _db.Customers
            .Where(c => c.Id == command.Id && c.RemovedAt == null)
            .FirstOrDefaultAsync();

        if (customer == null)
            throw new KeyNotFoundException("Customer not found");

        customer.Name = command.Name;
        customer.TaxId = command.TaxId;
        customer.Address = command.Address;
        customer.ZipCode = command.ZipCode;
        customer.City = command.City;
        customer.Country = command.Country;
        customer.Www = command.Www;
        customer.Facebook = command.Facebook;
        customer.LastModifiedAt = _clock.Current();

        await _db.SaveChangesAsync();
    }
}