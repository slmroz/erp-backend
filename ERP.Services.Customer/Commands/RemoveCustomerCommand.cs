using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public sealed record RemoveCustomerCommand(int Id) : ICommand;