using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ERP.Tests.Currencies;
public class UpdateCurrencyListHandlerTests
{
    [Fact]
    public async Task UpdateCurrencyList_ShouldUpdateExistingAndAddNew()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Current()).Returns(DateTime.UtcNow);

        // Istniejący kurs
        context.Currencies.Add(new Currency
        {
            Id = 1,
            BaseCurrency = "USD",
            TargetCurrency = "PLN",
            Rate = 3.5m
        });
        await context.SaveChangesAsync();

        var jsonResponse = @"{""amount"":1.0,""base"":""USD"",""date"":""2026-02-24"",""rates"":{""PLN"":3.5835,""EUR"":0.84911}}";
        var webService = new Mock<IWebService>();
        webService.Setup(x => x.DownloadAsync(It.IsAny<string>())).ReturnsAsync(jsonResponse);

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c.GetSection("ExternalApiSettings"))
            .Returns(new Mock<IConfigurationSection>().Object);
        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(s => s["CurrencyUrl"]).Returns("https://api.example.com");
        var externalApiSection = new Mock<IConfigurationSection>();
        externalApiSection.Setup(s => s["CurrencyUrl"]).Returns("https://api.example.com");

        configuration.Setup(c => c.GetSection("ExternalApiSettings"))
            .Returns(externalApiSection.Object);

        var handler = new UpdateCurrencyListHandler(configuration.Object, context, webService.Object, clock.Object);
        var command = new UpdateCurrencyListCommand();

        // Act
        await handler.HandleAsync(command);

        // Assert
        var pln = await context.Currencies.FirstAsync(c => c.TargetCurrency == "PLN");
        var eur = await context.Currencies.FirstAsync(c => c.TargetCurrency == "EUR");

        pln.Rate.Should().Be(3.5835m);
        pln.LastUpdatedAt.Should().NotBeNull();
        eur.Rate.Should().Be(0.84911m);
        eur.CreatedAt.Should().NotBeNull();
    }
}
