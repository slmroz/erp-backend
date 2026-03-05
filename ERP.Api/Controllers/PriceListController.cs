using ERP.Services.Abstractions.CQRS;
using Microsoft.AspNetCore.Mvc;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.Commands;
using ERP.Services.Products.DTO;
using ERP.Services.Products.Queries;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceListController : ControllerBase
{
    private readonly ICommandHandler<AddPriceListCommand> _addPriceListHandler;
    private readonly ICommandHandler<UpdatePriceListCommand> _updatePriceListHandler;
    private readonly ICommandHandler<RemovePriceListCommand> _removePriceListHandler;
    private readonly IQueryHandler<GetPriceListQuery, PriceListDto> _getPriceListHandler;
    private readonly IQueryHandler<GetPriceListsQuery, PagedResult<PriceListDto>> _getPriceListsHandler;

    public PriceListController(
        ICommandHandler<AddPriceListCommand> addPriceListHandler,
        ICommandHandler<UpdatePriceListCommand> updatePriceListHandler,
        ICommandHandler<RemovePriceListCommand> removePriceListHandler,
        IQueryHandler<GetPriceListQuery, PriceListDto> getPriceListHandler,
        IQueryHandler<GetPriceListsQuery, PagedResult<PriceListDto>> getPriceListsHandler)
    {
        _addPriceListHandler = addPriceListHandler;
        _updatePriceListHandler = updatePriceListHandler;
        _removePriceListHandler = removePriceListHandler;
        _getPriceListHandler = getPriceListHandler;
        _getPriceListsHandler = getPriceListsHandler;
    }

    /// <summary>
    /// Pobierz pojedynczą listę cen z licznikiem pozycji
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PriceListDto>> Get(int id)
    {
        var priceList = await _getPriceListHandler.HandleAsync(new GetPriceListQuery(id));
        return priceList != null ? Ok(priceList) : NotFound();
    }

    /// <summary>
    /// Paginowana lista list cen z wyszukiwaniem i licznikiem pozycji
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<PriceListDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortOrder = "asc")
    {
        var result = await _getPriceListsHandler.HandleAsync(
            new GetPriceListsQuery(page, pageSize, search, sortBy, sortOrder));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj nową listę cen
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AddPriceListCommand command)
    {
        await _addPriceListHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, command);
    }

    /// <summary>
    /// Aktualizuj listę cen
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePriceListCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updatePriceListHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń listę cen (soft delete - tylko bez pozycji)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, [FromQuery] int removedBy)
    {
        await _removePriceListHandler.HandleAsync(new RemovePriceListCommand(id, removedBy));
        return NoContent();
    }
}
