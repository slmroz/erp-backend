using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public sealed class AddContactCommand : ICommand
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNo { get; set; }
    public string? Email { get; set; }
}