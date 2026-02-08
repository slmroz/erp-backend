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