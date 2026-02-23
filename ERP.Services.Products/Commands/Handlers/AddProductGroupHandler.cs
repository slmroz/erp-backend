using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Products.Commands.Handlers;

internal sealed class AddProductGroupHandler : ICommandHandler<AddProductGroupCommand>
{
    private readonly ErpContext _dbContext;

    public AddProductGroupHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task HandleAsync(AddProductGroupCommand command)
    {
        // Walidacja unikalności Name
        if (await _dbContext.ProductGroups.AnyAsync(g => g.Name == command.Name && g.RemovedAt == null))
            throw new InvalidOperationException("ProductGroup name already exists");

        var group = new ProductGroup
        {
            Name = command.Name,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ProductGroups.Add(group);
        await _dbContext.SaveChangesAsync();
        command.Id = group.Id;
    }
}

