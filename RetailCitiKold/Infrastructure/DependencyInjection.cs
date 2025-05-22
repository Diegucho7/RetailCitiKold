using RetailCitiKold.Infrastructure.Extensions;

namespace RetailCitiKold.Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        
        builder.Services
            .AddDbContexts(builder.Configuration)
            .AddApiVersioningExtension()
            .AddCorsExtension()
            .AddAuthenticationMiddleware()
            .AddAuthorizationMiddleware()
            .AddCarterModules();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        return builder;
    }
    
    public static WebApplication ConfigureApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApiVersioningExtension();
        app.UseCorsExtension();
        app.UseAuthenticationMiddleware();
        app.UseAuthorizationMiddleware();
        
        app.UseHttpsRedirection();

        app.UseCarterModules();

        return app;
    }
}