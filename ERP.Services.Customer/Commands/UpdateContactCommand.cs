using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public record UpdateContactCommand(int Id, int? CustomerId, string FirstName, string LastName, string? PhoneNo, string? Email) : ICommand;