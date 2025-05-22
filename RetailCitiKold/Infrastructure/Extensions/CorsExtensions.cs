namespace IntegracionERP.Infrastructure.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsExtension(this IServiceCollection services)
    {   
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddCors();
        return services;
    }
    
    public static WebApplication UseCorsExtension(this WebApplication app)
    {
        app.UseCors(cors => cors
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true) // allow any origin
            .AllowCredentials() // allow credentials
            .WithExposedHeaders()
        );
        
        return app;
    }
}