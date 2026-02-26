using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;
public record GetCurrenciesQuery(
    int Page = 1,
    int PageSize = 20,
    string? BaseCurrency = null,
    string? TargetCurrency = null) : IQuery<PagedResult<CurrencyDto>>;