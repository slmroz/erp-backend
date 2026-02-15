using ERP.Services.Abstractions.CQRS;
using ERP.Services.Customer.DTO;

namespace ERP.Services.Customer.Commands;
public record GetContactQuery(int Id) : IQuery<ContactDto>;