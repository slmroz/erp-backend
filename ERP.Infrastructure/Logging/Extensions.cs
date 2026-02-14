using ERP.Infrastructure.Logging.Decorators;
using ERP.Services.Abstractions.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure.Logging;
internal static class Extensions
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));

        return services;
    }
}