namespace ERP.Services.Customer.DTO;
public record ContactDto(
    int Id,
    int? CustomerId,
    string FirstName,
    string LastName,
    string? PhoneNo,
    string? Email,
    string? CustomerName);
