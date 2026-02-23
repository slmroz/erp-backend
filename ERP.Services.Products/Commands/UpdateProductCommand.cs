using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record UpdateProductCommand(
    int Id,
    int ProductGroupId,
    string PartNumber,
    string Name,
    string? Description,
    string? OemBrand,
    decimal? ListPrice,
    decimal? WeightKg) : ICommand;
