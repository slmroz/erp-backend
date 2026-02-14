using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public sealed class AddCustomerCommand : ICommand
{
    public int? Id { get; set; }
    public string Name { get; set; } = default!;
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Www { get; set; }
    public string? Facebook { get; set; }
}