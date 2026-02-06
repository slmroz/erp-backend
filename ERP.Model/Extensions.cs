using ERP.Model.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Model;

public static class Extensions
{
    private const string OptionsSectionName = "Db";

    public static IServiceCollection AddModel(this IServiceCollection services, IConfiguration configuration)
    {
        // Fix: Use Bind to configure DbOptions instead of passing IConfigurationSection directly
        services.Configure<DbOptions>(options => configuration.GetRequiredSection(OptionsSectionName).Bind(options));
        var dbOptions = configuration.GetOptions<DbOptions>(OptionsSectionName);
        services.AddDbContext<ErpContext>(x => x.UseSqlServer(dbOptions.ConnectionString, o => o.UseCompatibilityLevel(110)));

        return services;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}
