using ERP.Services.Abstractions;
using ERP.Services.Customer.DTO;

namespace ERP.Services.Customer.Queries;

public class GetCustomerQuery : IQuery<CustomerDto>
{
    public int Id { get; set; }
}