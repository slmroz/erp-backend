using ERP.Services.Abstractions;

namespace ERP.Services.Customer.Commands;
public sealed record RemoveCustomerCommand(int Id) : ICommand;