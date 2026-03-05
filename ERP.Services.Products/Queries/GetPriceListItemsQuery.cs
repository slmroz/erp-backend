using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;

public record GetPriceListItemsQuery(
    int? PriceListId = null,       
    decimal? MinPrice = null,      
    decimal? MaxPrice = null,      
    int Page = 1,
    int PageSize = 20,
    string SortBy = "PriceListName",  
    string SortOrder = "asc")         
    : IQuery<PagedResult<PriceListItemDto>>;
