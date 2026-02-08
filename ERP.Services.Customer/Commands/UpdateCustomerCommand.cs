using ERP.Services.Abstractions;

namespace ERP.Services.Customer.Commands;
public sealed record UpdateCustomerCommand(
    int Id,
    string Name,
    string? TaxId,
    string? Address,
    string? ZipCode,
    string? City,
    string? Country,
    string? Www,
    string? Facebook
) : ICommand;