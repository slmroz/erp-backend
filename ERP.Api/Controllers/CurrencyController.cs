using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.Commands;
using ERP.Services.Products.DTO;
using ERP.Services.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ERP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICommandHandler<UpdateCurrencyListCommand> _updateCurrencyListHandler;
    private readonly IQueryHandler<GetCurrencyQuery, CurrencyDto> _getCurrencyHandler;
    private readonly IQueryHandler<GetCurrenciesQuery, PagedResult<CurrencyDto>> _getCurrenciesHandler;
    private readonly ICommandHandler<AddCurrencyCommand> _addCurrencyHandler;
    private readonly ICommandHandler<UpdateCurrencyCommand> _updateCurrencyHandler;
    private readonly ICommandHandler<RemoveCurrencyCommand> _removeCurrencyHandler;


    public CurrencyController(ICommandHandler<UpdateCurrencyListCommand> updateCurrencyListHandler,
        IQueryHandler<GetCurrencyQuery, CurrencyDto> getCurrencyHandler,
        IQueryHandler<GetCurrenciesQuery, PagedResult<CurrencyDto>> getCurrenciesHandler,
        ICommandHandler<AddCurrencyCommand> addCurrencyHandler,
        ICommandHandler<UpdateCurrencyCommand> updateCurrencyHandler,
        ICommandHandler<RemoveCurrencyCommand> removeCurrencyHandler)
    {
        _updateCurrencyListHandler = updateCurrencyListHandler;
        _getCurrencyHandler = getCurrencyHandler;
        _getCurrenciesHandler = getCurrenciesHandler;
        _addCurrencyHandler = addCurrencyHandler;
        _updateCurrencyHandler = updateCurrencyHandler;
        _removeCurrencyHandler = removeCurrencyHandler;
    }

    [HttpPost("Refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateList([FromBody] UpdateCurrencyListCommand command)
    {
        await _updateCurrencyListHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Pobierz pojedynczy kurs waluty
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CurrencyDto>> Get(int id)
    {
        var currency = await _getCurrencyHandler.HandleAsync(new GetCurrencyQuery(id));
        return currency != null ? Ok(currency) : NotFound();
    }

    /// <summary>
    /// Paginowana lista kursów walut z filtrem Base/Target + refresh z API
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CurrencyDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? baseCurrency = null,
        [FromQuery] string? targetCurrency = null,
        [FromQuery] bool? refresh = null,        // Zachowane
        [FromQuery] string sortBy = "targetcurrency",  
        [FromQuery] string sortOrder = "asc")       
    {
        // ✅ Refresh z API (bez await - fire & forget)
        if (refresh == true)
        {
            _ = Task.Run(async () =>
                await _updateCurrencyListHandler.HandleAsync(new UpdateCurrencyListCommand()));
        }

        var result = await _getCurrenciesHandler.HandleAsync(
            new GetCurrenciesQuery(page, pageSize, baseCurrency, targetCurrency, sortBy, sortOrder));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj nowy kurs waluty (z API)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create([FromBody] AddCurrencyCommand command)
    {
        await _addCurrencyHandler.HandleAsync(command);
        var currencyId = command.Id;

        return CreatedAtAction(nameof(Get), new { id = currencyId }, currencyId);
    }

    /// <summary>
    /// Aktualizuj kurs waluty (nowy rate z API)
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCurrencyCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateCurrencyHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń kurs waluty (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeCurrencyHandler.HandleAsync(new RemoveCurrencyCommand(id));
        return NoContent();
    }
}
