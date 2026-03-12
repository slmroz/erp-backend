using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Customer.Commands.Handlers;
internal sealed class AddInquiryLeadHandler : ICommandHandler<AddInquiryLeadCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;
    private readonly IEmailService _emailSender;  // Powiadomienie Sales

    public AddInquiryLeadHandler(ErpContext dbContext, IClock clock, IEmailService emailSender)
    {
        _dbContext = dbContext;
        _clock = clock;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(AddInquiryLeadCommand command)
    {
        IDbContextTransaction? transaction = null;

        if (!_dbContext.Database.IsInMemory())
        {
            transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        try
        {
            // 1. Utwórz Customer
            var customer = new Model.Model.Customer
            {
                Name = command.CustomerName,
                TaxId = command.CustomerTaxId,
                CreatedAt = _clock.Current()
            };
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            // 2. Utwórz Contact
            var contact = new Contact
            {
                CustomerId = customer.Id,
                FirstName = command.ContactFirstName,
                LastName = command.ContactLastName,
                Email = command.ContactEmail,
                PhoneNo = command.ContactPhone,
                CreatedAt = _clock.Current()
            };
            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();

            // 3. Utwórz Lead
            var lead = new Lead
            {
                CustomerId = customer.Id,
                ContactId = contact.Id,
                Subject = command.Subject,
                Description = command.Description,
                LeadSourceId = (int)Model.Enum.LeadSource.WebSite,
                StatusId = (int)Model.Enum.LeadStatus.New,
                PriorityId = (int)command.Priority,
                ExpectedResponseDate = DateOnly.FromDateTime(_clock.Current().AddDays(1)),
                CreatedAt = _clock.Current(),
                CreatedBy = 0
            };
            await _dbContext.Leads.AddAsync(lead);
            await _dbContext.SaveChangesAsync();

            if(transaction != null)
            await transaction.CommitAsync();

            // 4. Powiadomienie Sales Team
            var model = new Dictionary<string, string>();
            model.Add("Subject", command.Subject);
            model.Add("CustomerName", command.CustomerName);

            await _emailSender.SendAsync("Sales", "InquiryAlert", model);

            command.Id = lead.Id;
        }
        catch
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            throw;
        }
    }
}
