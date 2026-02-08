using ERP.Model.Model;
using ERP.Services.Abstractions;

namespace ERP.Services.Customer.Commands.Handlers;
public sealed class AddCustomerHandler : ICommandHandler<AddCustomerCommand>
{
    private readonly ErpContext _db;

    public AddCustomerHandler(ErpContext db)
    {
        _db = db;
    }

    public async Task HandleAsync(AddCustomerCommand command)
    {
        var customer = new ERP.Model.Model.Customer
        {
            Name = command.Name,
            TaxId = command.TaxId,
            Address = command.Address,
            ZipCode = command.ZipCode,
            City = command.City,
            Country = command.Country,
            Www = command.Www,
            Facebook = command.Facebook,
            CreatedAt = DateTime.UtcNow
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        command.Id = customer.Id;
    }
}