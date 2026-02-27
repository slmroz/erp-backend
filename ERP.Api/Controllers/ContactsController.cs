using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;
[ApiController]
[Route("contacts")]
public class ContactsController : ControllerBase
{
    private readonly IQueryHandler<GetContactQuery, ContactDto> _getContactHandler;
    private readonly IQueryHandler<GetContactsQuery, PagedResult<ContactDto>> _getContactsHandler;
    private readonly ICommandHandler<AddContactCommand> _addContactHandler;
    private readonly ICommandHandler<UpdateContactCommand> _updateContactHandler;
    private readonly ICommandHandler<RemoveContactCommand> _removeContactHandler;

    public ContactsController(
        IQueryHandler<GetContactQuery, ContactDto> getContact,
        IQueryHandler<GetContactsQuery, PagedResult<ContactDto>> getContacts,
        ICommandHandler<AddContactCommand> addContact,
        ICommandHandler<UpdateContactCommand> updateContact,
        ICommandHandler<RemoveContactCommand> removeContact)
    {
        _getContactHandler = getContact;
        _getContactsHandler = getContacts;
        _addContactHandler = addContact;
        _updateContactHandler = updateContact;
        _removeContactHandler = removeContact;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContactDto>> Get(int id)
    {
        var contact = await _getContactHandler.HandleAsync(new GetContactQuery(id));
        return contact != null ? Ok(contact) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ContactDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? customerId = null,
        [FromQuery] string sortBy = "lastname",    
        [FromQuery] string sortOrder = "asc")      
    {
        var result = await _getContactsHandler.HandleAsync(
            new GetContactsQuery(page, pageSize, search, customerId, sortBy, sortOrder));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create([FromBody] AddContactCommand command)
    {
        await _addContactHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, command.Id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContactCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateContactHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeContactHandler.HandleAsync(new RemoveContactCommand(id));
        return NoContent();
    }
}
