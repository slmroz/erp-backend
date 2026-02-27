using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.DTO;
using ERP.Services.Customer.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IQueryHandler<GetCustomerQuery, CustomerDto> _getCustomerHandler;
    private readonly IQueryHandler<GetCustomersQuery, PagedResult<CustomerDto>> _getCustomersHandler;
    private readonly ICommandHandler<AddCustomerCommand> _addCustomerHandler;
    private readonly ICommandHandler<UpdateCustomerCommand> _updateCustomerHandler;
    private readonly ICommandHandler<RemoveCustomerCommand> _removeCustomerHandler;

    public CustomersController(
        IQueryHandler<GetCustomerQuery, CustomerDto> getCustomer,
        IQueryHandler<GetCustomersQuery, PagedResult<CustomerDto>> getCustomers,
        ICommandHandler<AddCustomerCommand> addCustomer,
        ICommandHandler<UpdateCustomerCommand> updateCustomer,
        ICommandHandler<RemoveCustomerCommand> removeCustomer)
    {
        _getCustomerHandler = getCustomer;
        _getCustomersHandler = getCustomers;
        _addCustomerHandler = addCustomer;
        _updateCustomerHandler = updateCustomer;
        _removeCustomerHandler = removeCustomer;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Get(int id)
    {
        var customer = await _getCustomerHandler.HandleAsync(new GetCustomerQuery() { Id = id });
        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string sortBy = "name",       
        [FromQuery] string sortOrder = "asc")
    {
        var result = await _getCustomersHandler.HandleAsync(
            new GetCustomersQuery(page, pageSize, search));

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AddCustomerCommand command)
    {
        await _addCustomerHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { command.Id }, command.Id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _updateCustomerHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeCustomerHandler.HandleAsync(new RemoveCustomerCommand(id));
        return NoContent();
    }
}
