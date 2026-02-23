using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public record RemoveProductGroupCommand(int Id) : ICommand;

