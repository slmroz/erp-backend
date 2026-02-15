using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Commands.Handlers;
internal sealed class UpdateContactHandler : ICommandHandler<UpdateContactCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _db;

    public UpdateContactHandler(ErpContext db, IClock clock)
    {
        _clock = clock;
        _db = db;
    }

    public async Task HandleAsync(UpdateContactCommand command)
    {
        var contact = await _db.Contacts
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        if (contact == null)
            throw new KeyNotFoundException("Contact not found");

        // Walidacja CustomerId
        if (command.CustomerId.HasValue && command.CustomerId != contact.CustomerId)
        {
            var customerExists = await _db.Customers
                .AnyAsync(c => c.Id == command.CustomerId.Value && c.RemovedAt == null);
            if (!customerExists)
                throw new KeyNotFoundException($"Customer {command.CustomerId} not found");
        }

        contact.CustomerId = command.CustomerId;
        contact.FirstName = command.FirstName;
        contact.LastName = command.LastName;
        contact.PhoneNo = command.PhoneNo;
        contact.Email = command.Email;

        await _db.SaveChangesAsync();
    }
}