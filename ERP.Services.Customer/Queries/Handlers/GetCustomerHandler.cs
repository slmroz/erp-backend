using ERP.Model.Model;
using ERP.Services.Abstractions;
using ERP.Services.Customer.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Queries.Handlers;
public sealed class GetCustomerHandler : IQueryHandler<GetCustomerQuery, CustomerDto>
{
    private readonly ErpContext _db;

    public GetCustomerHandler(ErpContext db)
    {
        _db = db;
    }

    public Task<CustomerDto?> HandleAsync(GetCustomerQuery query)
    {
        return _db.Customers
            .AsNoTracking()
            .Where(c => c.Id == query.Id && c.RemovedAt == null)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                TaxId = c.TaxId,
                Address = c.Address,
                ZipCode = c.ZipCode,
                City = c.City,
                Country = c.Country,
                Www = c.Www,
                Facebook = c.Facebook
            })
            .FirstOrDefaultAsync();
    }
}