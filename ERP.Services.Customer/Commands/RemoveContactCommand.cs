using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public record RemoveContactCommand(int Id) : ICommand;
