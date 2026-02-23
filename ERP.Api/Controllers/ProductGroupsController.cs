using Microsoft.AspNetCore.Mvc;

using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.Queries;
using ERP.Services.Products.DTO;
using ERP.Services.Products.Commands;
using ERP.Services.Abstractions.Search;

namespace ERP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductGroupsController : ControllerBase
{
    private readonly IQueryHandler<GetProductGroupQuery, ProductGroupDto> _getProductGroupHandler;
    private readonly IQueryHandler<GetProductGroupsQuery, PagedResult<ProductGroupDto>> _getProductGroupsHandler;
    private readonly ICommandHandler<AddProductGroupCommand> _addProductGroupHandler;
    private readonly ICommandHandler<UpdateProductGroupCommand> _updateProductGroupHandler;
    private readonly ICommandHandler<RemoveProductGroupCommand> _removeProductGroupHandler;

    public ProductGroupsController(
        IQueryHandler<GetProductGroupQuery, ProductGroupDto> getProductGroupHandler,
        IQueryHandler<GetProductGroupsQuery, PagedResult<ProductGroupDto>> getProductGroupsHandler,
        ICommandHandler<AddProductGroupCommand> addProductGroupHandler,
        ICommandHandler<UpdateProductGroupCommand> updateProductGroupHandler,
        ICommandHandler<RemoveProductGroupCommand> removeProductGroupHandler)
    {
        _getProductGroupHandler = getProductGroupHandler;
        _getProductGroupsHandler = getProductGroupsHandler;
        _addProductGroupHandler = addProductGroupHandler;
        _updateProductGroupHandler = updateProductGroupHandler;
        _removeProductGroupHandler = removeProductGroupHandler;
    }

    /// <summary>
    /// Pobierz pojedynczą grupę produktów z licznikiem produktów
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductGroupDto>> Get(int id)
    {
        var group = await _getProductGroupHandler.HandleAsync(new GetProductGroupQuery(id));
        return group != null ? Ok(group) : NotFound();
    }

    /// <summary>
    /// Paginowana lista grup produktów z wyszukiwaniem i licznikiem produktów
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductGroupDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var result = await _getProductGroupsHandler.HandleAsync(
            new GetProductGroupsQuery(page, pageSize, search));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj nową grupę produktów Automotive Parts
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create([FromBody] AddProductGroupCommand command)
    {
        await _addProductGroupHandler.HandleAsync(command);
        var groupId = command.Id;
        return CreatedAtAction(nameof(Get), new { id = groupId }, groupId);
    }

    /// <summary>
    /// Aktualizuj grupę produktów
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductGroupCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateProductGroupHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń grupę produktów (soft delete - tylko bez aktywnych produktów)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeProductGroupHandler.HandleAsync(new RemoveProductGroupCommand(id));
        return NoContent();
    }
}

