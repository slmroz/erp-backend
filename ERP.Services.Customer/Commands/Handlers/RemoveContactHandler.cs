using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Commands.Handlers;
public sealed class RemoveContactHandler : ICommandHandler<RemoveContactCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _db;

    public RemoveContactHandler(ErpContext db, IClock clock)
    {
        _clock = clock;
        _db = db;
    }

    public async Task HandleAsync(RemoveContactCommand command)
    {
        var contact = await _db.Contacts
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        if (contact == null)
            throw new KeyNotFoundException("Contact not found");

        // Soft delete
        contact.RemovedAt = _clock.Current();
        await _db.SaveChangesAsync();
    }
}