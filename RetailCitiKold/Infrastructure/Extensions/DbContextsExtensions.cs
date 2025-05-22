using IntegracionERP.Domain.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IntegracionERP.Infrastructure.Extensions;

public static class DbContextsExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var eProductionConnectionString =
            Environment.GetEnvironmentVariable("EPRODUCTION_CONNECTION_STRING")
            ?? configuration.GetConnectionString("EProductionConnection");
        var integrationConnectionString =
            Environment.GetEnvironmentVariable("INTEGRATION_CONNECTION_STRING")
            ?? configuration.GetConnectionString("IntegrationConnection");
        
        ArgumentNullException.ThrowIfNull(eProductionConnectionString);
        ArgumentNullException.ThrowIfNull(integrationConnectionString);
        
        services.AddDbContext<Domain.DataAccess.Context.RetailCitiKold>(options =>
        {
            options.UseSqlServer(eProductionConnectionString, builder =>
            {
                //TODO: builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            });
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        services.AddDbContext<IntegrationDbContext>(options =>
        {
            options.UseSqlServer(integrationConnectionString, builder =>
            {
                //TODO: builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            });
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
        

        return services;
    }
}