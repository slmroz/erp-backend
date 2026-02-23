using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;
public record GetProductGroupsQuery(int Page = 1, int PageSize = 20, string? Search = null) : IQuery<PagedResult<ProductGroupDto>>;
