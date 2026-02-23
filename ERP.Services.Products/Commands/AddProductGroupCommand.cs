using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;

public class AddProductGroupCommand : ICommand
{
    public int Id { get; set; }

    public string Name { get; }
    public string? Description { get; }

    public AddProductGroupCommand(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}

