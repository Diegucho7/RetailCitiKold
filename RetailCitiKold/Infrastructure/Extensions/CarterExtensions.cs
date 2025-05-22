using Carter;

namespace IntegracionERP.Infrastructure.Extensions;

public static class CarterExtensions
{
    public static IServiceCollection AddCarterModules(this IServiceCollection services)
    {
        var assemblies = new[] { AssemblyReference.Assembly };
        services.AddCarter(new DependencyContextAssemblyCatalog(assemblies));
        return services;
    }
    
    public static WebApplication UseCarterModules(this WebApplication app)
    {
        return app;
    }
}