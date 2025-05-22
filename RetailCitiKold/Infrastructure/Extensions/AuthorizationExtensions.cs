namespace IntegracionERP.Infrastructure.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationMiddleware(this IServiceCollection services)
    {   
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddAuthorization();

        return services;
    }
    
    public static WebApplication UseAuthorizationMiddleware(this WebApplication app)
    {
        app.UseAuthorization();
        
        return app;
    }
}