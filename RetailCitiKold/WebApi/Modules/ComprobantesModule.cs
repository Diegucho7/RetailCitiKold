using Carter;
using RetailCitiKold.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace RetailCitiKold.WebApi.Modules;

public class ComprobantesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var comprobantes = app.NewVersionedApi("Comprobantes");
        var group = comprobantes.MapGroup("api/v{version:apiVersion}")
            .HasApiVersion(1.0);

        group.MapPost("/asentar/{id}/{tipo}", Asentar);
        
        group.MapPost("/reversar/{id}/{tipo}", Reversar);
        
        group.MapPost("/anular/{id}/{tipo}", Anular);
        
        group.MapPost("/pagos", Pagos);
        
        group.MapPost("/retenciones", Retenciones);
        
        group.MapPost("/compras-cobros", ComprasCobros);
        
        // .Produces<LoginResponse>()
        // .Produces<Result>((int)HttpStatusCode.BadRequest)
        // .Produces<Result>((int)HttpStatusCode.Unauthorized)
        // .Produces<Result>((int)HttpStatusCode.TooManyRequests)
        // .Produces<Result>((int)HttpStatusCode.InternalServerError)
        // .AllowAnonymous();
    }

    private static Task Asentar(
        [FromRoute] int id,
        [FromRoute] string tipo,
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
    
    private static Task Reversar(
        [FromRoute] int id,
        [FromRoute] string tipo,
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
    
    private static Task Anular(
        [FromRoute] int id,
        [FromRoute] string tipo,
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
    
    private static Task Pagos(
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
    
    private static Task Retenciones(
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
    
    private static Task ComprasCobros(
        [FromServices] IIntegracionErpService service
    ) => Task.CompletedTask;
}