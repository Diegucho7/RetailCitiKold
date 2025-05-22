using Asp.Versioning;

namespace IntegracionERP.Infrastructure.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningExtension(this IServiceCollection services)
    {   
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1,0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader(),
                    new HeaderApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader(),
                    new UrlSegmentApiVersionReader()
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .EnableApiVersionBinding();
        
        return services;
    }
    
    public static WebApplication UseApiVersioningExtension(this WebApplication app) 
    {   
        return app;
    }
}