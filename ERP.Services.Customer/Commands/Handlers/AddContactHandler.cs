using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Commands.Handlers;
public sealed class AddContactHandler : ICommandHandler<AddContactCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _db;

    public AddContactHandler(ErpContext db, IClock clock)
    {
        _clock = clock;
        _db = db;
    }

    public async Task HandleAsync(AddContactCommand command)
    {
        // Opcjonalna walidacja: czy CustomerId istnieje?
        if (command.CustomerId.HasValue)
        {
            var customerExists = await _db.Customers
                .AnyAsync(c => c.Id == command.CustomerId.Value && c.RemovedAt == null);
            if (!customerExists)
                throw new KeyNotFoundException($"Customer {command.CustomerId} not found");
        }

        var contact = new Contact
        {
            CustomerId = command.CustomerId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PhoneNo = command.PhoneNo,
            Email = command.Email
        };

        await _db.Contacts.AddAsync(contact);
        await _db.SaveChangesAsync();

        command.Id = contact.Id;
    }
}
