namespace ERP.Services.Customer.DTO;
public sealed class CustomerDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? TaxId { get; init; }
    public string? Address { get; init; }
    public string? ZipCode { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? Www { get; init; }
    public string? Facebook { get; init; }
}