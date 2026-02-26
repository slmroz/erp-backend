using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ERP.Services.Products.Commands.Handlers;

internal sealed class UpdateCurrencyListHandler : ICommandHandler<UpdateCurrencyListCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _dbContext;
    private readonly IWebService _webService;
    private readonly IConfiguration _configuration;
    public UpdateCurrencyListHandler(IConfiguration configuration, ErpContext dbContext, IWebService webService, IClock clock) { 
        _dbContext = dbContext;
        _webService = webService;
        _configuration = configuration;
        _clock = clock;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task HandleAsync(UpdateCurrencyListCommand command)
    {
        var externalApiSettings = _configuration.GetSection("ExternalApiSettings");
        var currencyUrl = externalApiSettings["CurrencyUrl"];

        var response = await _webService.DownloadAsync(currencyUrl);

        var currencyRates = JsonSerializer.Deserialize<CurrencyRatesResponseDto>(response, _jsonOptions)
            ?? throw new JsonException("Invalid currency rates JSON");

        foreach (var currencyRate in currencyRates.Rates)
        {
            var dbRate = await _dbContext.Currencies.FirstOrDefaultAsync(c => c.TargetCurrency == currencyRate.Key && c.BaseCurrency == currencyRates.Base).ConfigureAwait(false);
            if (dbRate != null)
            {
                dbRate.Rate = currencyRate.Value;
                dbRate.LastUpdatedAt = _clock.Current();
            }
            else {
                dbRate = new Currency() {
                    TargetCurrency = currencyRate.Key,
                    BaseCurrency = currencyRates.Base,
                    CreatedAt = _clock.Current(),
                    LastUpdatedAt = _clock.Current(),
                    Rate = currencyRate.Value
                };
                await _dbContext.Currencies.AddAsync(dbRate);
            }
        }
        await _dbContext.SaveChangesAsync();
    }
}