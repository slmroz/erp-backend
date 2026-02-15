using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Queries.Handlers;
internal sealed class GetContactHandler : IQueryHandler<GetContactQuery, ContactDto>
{
    private readonly ErpContext _db;

    public GetContactHandler(ErpContext db)
    {
        _db = db;
    }

    public async Task<ContactDto?> HandleAsync(GetContactQuery query)
    {
        var contact = await _db.Contacts
            .Where(c => c.RemovedAt == null)
            .Include(c => c.Customer)
            .Select(c => new ContactDto(
                c.Id,
                c.CustomerId,
                c.FirstName!,
                c.LastName!,
                c.PhoneNo,
                c.Email,
                c.Customer != null ? c.Customer.Name : null
            ))
            .FirstOrDefaultAsync(c => c.Id == query.Id);

        return contact;
    }
}
