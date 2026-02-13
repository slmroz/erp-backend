using ERP.Infrastructure.Auth;
using ERP.Infrastructure.CommonServices;
using ERP.Infrastructure.Config;
using ERP.Infrastructure.Exceptions;
using ERP.Infrastructure.Logging;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.Time;
using ERP.Model.Abstractions;
using ERP.Services.Common;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ERP.Tests")]
namespace ERP.Infrastructure;


public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<AppOptions>(configuration.GetRequiredSection(ConfigSection.app.ToString()));
        services.AddSingleton<ExceptionMiddleware>();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddSingleton<IClock, Clock>();

        services.AddCustomLogging();
        services.AddSecurity();
        services.AddEndpointsApiExplorer();

        services.AddScoped<IEmailService, EmailService>();

        var infrastructureAssembly = typeof(AppOptions).Assembly;

        services.AddAuth(configuration);

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (exception is ValidationException validation)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        errors = validation.Errors.Select(e => new
                        {
                            e.PropertyName,
                            e.ErrorMessage
                        })
                    });
                }
            });
        });

        return app;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}