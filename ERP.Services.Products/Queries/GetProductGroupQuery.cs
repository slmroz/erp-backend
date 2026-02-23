using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;
public record GetProductGroupQuery(int Id) : IQuery<ProductGroupDto>;

