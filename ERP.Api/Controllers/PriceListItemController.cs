using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.Commands;
using ERP.Services.Products.DTO;
using ERP.Services.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceListItemController : ControllerBase
{
    private readonly ICommandHandler<AddPriceListItemCommand> _addPriceListItemHandler;
    private readonly ICommandHandler<UpdatePriceListItemCommand> _updatePriceListItemHandler;
    private readonly ICommandHandler<RemovePriceListItemCommand> _removePriceListItemHandler;
    private readonly IQueryHandler<GetPriceListItemQuery, PriceListItemDto> _getPriceListItemHandler;
    private readonly IQueryHandler<GetPriceListItemsQuery, PagedResult<PriceListItemDto>> _getPriceListItemsHandler;

    public PriceListItemController(
        ICommandHandler<AddPriceListItemCommand> addPriceListItemHandler,
        ICommandHandler<UpdatePriceListItemCommand> updatePriceListItemHandler,
        ICommandHandler<RemovePriceListItemCommand> removePriceListItemHandler,
        IQueryHandler<GetPriceListItemQuery, PriceListItemDto> getPriceListItemHandler,
        IQueryHandler<GetPriceListItemsQuery, PagedResult<PriceListItemDto>> getPriceListItemsHandler)
    {
        _addPriceListItemHandler = addPriceListItemHandler;
        _updatePriceListItemHandler = updatePriceListItemHandler;
        _removePriceListItemHandler = removePriceListItemHandler;
        _getPriceListItemHandler = getPriceListItemHandler;
        _getPriceListItemsHandler = getPriceListItemsHandler;
    }

    /// <summary>
    /// Pobierz pojedynczą pozycję cenową z nazwami listy/produktu
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PriceListItemDto>> Get(int id)
    {
        var item = await _getPriceListItemHandler.HandleAsync(new GetPriceListItemQuery(id));
        return item != null ? Ok(item) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PriceListItemDto>>> GetList(
        [FromQuery] int? priceListId = null,
        [FromQuery] decimal? minPrice = null,     
        [FromQuery] decimal? maxPrice = null,     
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "pricelistname",  
        [FromQuery] string sortOrder = "asc")         
    {
        var result = await _getPriceListItemsHandler.HandleAsync(
            new GetPriceListItemsQuery(priceListId, minPrice, maxPrice, page, pageSize, sortBy, sortOrder));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj pozycję cenową do listy
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AddPriceListItemCommand command)
    {
        await _addPriceListItemHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, command);
    }

    /// <summary>
    /// Aktualizuj cenę pozycji
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePriceListItemCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updatePriceListItemHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń pozycję cenową
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, [FromQuery] int removedBy)
    {
        await _removePriceListItemHandler.HandleAsync(new RemovePriceListItemCommand(id, removedBy));
        return NoContent();
    }
}
