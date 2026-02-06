using ERP.Model.Model;
using ERP.Services.Abstractions.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure.Security;
internal static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services
            .AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddSingleton<IPasswordManager, PasswordManager>();

        return services;
    }
}
