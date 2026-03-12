using ERP.Model.Enum;
using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Customer.Commands;
public class AddInquiryLeadCommand : ICommand
{
    public AddInquiryLeadCommand(
    string Subject,
    string Description,
    string CustomerName,
    string CustomerTaxId,
    string ContactFirstName,
    string ContactLastName,
    string? ContactEmail,
    string? ContactPhone,
    Priority Priority = Priority.Medium)
    {
        this.Subject = Subject;
        this.Description = Description;
        this.CustomerName = CustomerName;
        this.CustomerTaxId = CustomerTaxId;
        this.ContactFirstName = ContactFirstName;
        this.ContactLastName = ContactLastName;
        this.ContactEmail = ContactEmail;
        this.ContactPhone = ContactPhone;
        this.Priority = Priority;
    }

    public string Subject { get; private set; }
    public string Description { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerTaxId { get; private set; }
    public string ContactFirstName { get; private set; }
    public string ContactLastName { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public Priority Priority { get; private set; }

    public int Id { get; set; }
}
