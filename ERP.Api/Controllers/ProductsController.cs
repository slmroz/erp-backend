using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Queries;
using ERP.Services.Products.DTO;
using Microsoft.AspNetCore.Mvc;
using ERP.Services.Abstractions.Search;

namespace ERP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IQueryHandler<GetProductQuery, ProductDto> _getProductHandler;
    private readonly IQueryHandler<GetProductsQuery, PagedResult<ProductDto>> _getProductsHandler;
    private readonly ICommandHandler<AddProductCommand> _addProductHandler;
    private readonly ICommandHandler<UpdateProductCommand> _updateProductHandler;
    private readonly ICommandHandler<RemoveProductCommand> _removeProductHandler;

    public ProductsController(
        IQueryHandler<GetProductQuery, ProductDto> getProductHandler,
        IQueryHandler<GetProductsQuery, PagedResult<ProductDto>> getProductsHandler,
        ICommandHandler<AddProductCommand> addProductHandler,
        ICommandHandler<UpdateProductCommand> updateProductHandler,
        ICommandHandler<RemoveProductCommand> removeProductHandler)
    {
        _getProductHandler = getProductHandler;
        _getProductsHandler = getProductsHandler;
        _addProductHandler = addProductHandler;
        _updateProductHandler = updateProductHandler;
        _removeProductHandler = removeProductHandler;
    }

    /// <summary>
    /// Pobierz pojedynczy produkt z danymi grupy
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _getProductHandler.HandleAsync(new GetProductQuery(id));
        return product != null ? Ok(product) : NotFound();
    }

    /// <summary>
    /// Paginowana lista produktów z wyszukiwaniem i filtrem po grupie
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? groupId = null)
    {
        var result = await _getProductsHandler.HandleAsync(
            new GetProductsQuery(page, pageSize, search, groupId));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj nowy produkt Automotive Parts
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create([FromBody] AddProductCommand command)
    {
        await _addProductHandler.HandleAsync(command);
        var productId = command.Id;
        return CreatedAtAction(nameof(Get), new { id = productId }, productId);
    }

    /// <summary>
    /// Aktualizuj produkt (PartNumber, OEM, cena, waga)
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateProductHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń produkt (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeProductHandler.HandleAsync(new RemoveProductCommand(id));
        return NoContent();
    }
}
