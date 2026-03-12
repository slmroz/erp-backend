using ERP.Services.Abstractions.CQRS;
using ERP.Services.Customer.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LeadController : ControllerBase
{
    private readonly ICommandHandler<AddInquiryLeadCommand> _addInquiryLeadHandler;

    public LeadController(ICommandHandler<AddInquiryLeadCommand> addInquiryLeadHandler)
    {
        _addInquiryLeadHandler = addInquiryLeadHandler;
    }

    [HttpPost("inquiry")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> SubmitInquiry([FromBody] AddInquiryLeadCommand command)
    {
        await _addInquiryLeadHandler.HandleAsync(command);
        var leadId = command.Id;
        return CreatedAtAction(nameof(SubmitInquiry), new { id = leadId }, leadId);
    }
}
