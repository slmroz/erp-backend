using ERP.Model.Model;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests;
public static class TestDbContextFactory
{
    public static ErpContext Create()
    {
        var options = new DbContextOptionsBuilder<ErpContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ErpContext(options);
    }
}

public static class PriceListTestFactory
{
    public static PriceList CreatePriceList(string name, int currencyId, int? id = null)
    {
        return new PriceList
        {
            Id = id ?? 0,
            Name = name,
            Description = $"{name} description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = 1,
            CurrencyId = currencyId,
            DiscountPercentage = 0m,
            LastUpdatedAt = DateTime.UtcNow.AddDays(-1),  // ✅ BRAKOWAŁO!
            LastUpdatedBy = 1
        };
    }

    public static PriceListItem CreatePriceListItem(int priceListId, int productId, decimal price)
    {
        return new PriceListItem
        {
            Id = 0,
            PriceListId = priceListId,
            ProductId = productId,
            Price = price,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = 1,
            LastUpdatedAt = DateTime.UtcNow.AddDays(-1),  // ✅ Dodaj dla spójności
            LastUpdatedBy = 1
        };
    }
}
