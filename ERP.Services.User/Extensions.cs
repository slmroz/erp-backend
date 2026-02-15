using ERP.Services.Abstractions.CQRS;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ERP.Services.User;

public static class Extensions
{
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
        .WithScopedLifetime());

        return services;
    }
}