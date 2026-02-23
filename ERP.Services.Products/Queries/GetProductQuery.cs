using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;

public record GetProductQuery(int Id) : IQuery<ProductDto>;

