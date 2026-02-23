using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record UpdateProductGroupCommand(int Id, string Name, string? Description) : ICommand;

