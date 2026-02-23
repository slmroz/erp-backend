using ERP.Services.Abstractions.CQRS;

namespace ERP.Services.Products.Commands;
public class AddProductCommand : ICommand
{
    public AddProductCommand(int productGroupId, string name, string partNumber, string? description, string? oemBrand, decimal? listPrice, decimal? weightKg)
    {
        ProductGroupId = productGroupId;
        Name = name;
        PartNumber = partNumber;
        Description = description;
        OemBrand = oemBrand;
        ListPrice = listPrice;
        WeightKg = weightKg;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int ProductGroupId { get; set; }

    public string PartNumber { get; set; }
    public string? Description { get; set; }
    public string? OemBrand { get; set; }
    public decimal? ListPrice { get; set; }
    public decimal? WeightKg { get; set; }
}

