
using ERP.Services.Abstractions;
using ERP.Services.Abstractions.Search;
using ERP.Services.Customer.DTO;

namespace ERP.Services.Customer.Queries;
public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null
) : IQuery<PagedResult<CustomerDto>>;