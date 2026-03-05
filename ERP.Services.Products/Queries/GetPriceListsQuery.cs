using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;

public record GetPriceListsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string SortBy = "Name",      
    string SortOrder = "asc")    
    : IQuery<PagedResult<PriceListDto>>;
